using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using webAdmin.Models;
using webAdmin.Utilities;

namespace webAdmin.Services;


public interface IAuthenticationService
{
    Task<string?> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string firstName, string lastName, string email, string password);
    Task LogoutAsync();
    Task SignInWithTokenAsync(TokenContainer tokenContainer);
}
public class AuthenticationManager(IHttpClientFactory httpClientFactory,IHttpContextAccessor httpContextAccessor,
    TokenContainer tokenContainer) : IAuthenticationService
{

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ChatbotApi");
    private readonly TokenContainer _tokenContainer = tokenContainer;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<string?> LoginAsync(string email, string password)
    {
        var request = CreateHttpRequest(new { Email = email, Password = password });
        var response = await _httpClient.PostAsync("/identity/login", request);

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonSerializer.Deserialize<TokenContainer>(content);
        _tokenContainer.SetRefreshToken(tokenResponse.RefreshToken);
        _tokenContainer.SetAccessToken(tokenResponse.AccessToken);  
        _tokenContainer.SetExpiresIn(tokenResponse.ExpiresIn);  
        return tokenResponse.AccessToken;
    }

    public async Task<bool> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        var request = CreateHttpRequest(new {Email=email, Password = password, FirstName= firstName,LastName=lastName});
        var response = await _httpClient.PostAsync("/identiy/register", request);
        if (!response.IsSuccessStatusCode)
            return false;

        return true;

    }

    private static StringContent CreateHttpRequest(object data)
    {
        var json = JsonSerializer.Serialize(data);

        return new StringContent(json,Encoding.UTF8,"application/json");

    }

    public Task LogoutAsync()
    {
        _tokenContainer.AccessToken = string.Empty;
        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available.");
        return context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task SignInWithTokenAsync(TokenContainer tokenContainer)
    {
        var response = await _httpClient.GetFromJsonAsync<CurrentUserDto>("/account/me");
        if (response is null)
            return;
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, response.Id.ToString()),
            new(ClaimTypes.Email, response.Email),
            new(ClaimTypes.Name, response.Email)
        };
        claims.AddRange(response.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var expiration = tokenContainer.ExpiresIn;
        var principal = new ClaimsPrincipal(identity);
        var context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");
        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.AddSeconds(expiration),
            }
        );


    }
}
