using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using webAdmin.Models;
using webAdmin.Utilities;

namespace webAdmin.Services;


public interface IAuthenticationService
{
    Task<bool?> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterDto model);
    Task LogoutAsync();
    Task SignInWithTokenAsync();

    bool IsAuthenticated { get; set; }
    CurrentUserDto CurrentUser { get; set; }
}
public class AuthenticationManager(IHttpClientFactory httpClientFactory,IHttpContextAccessor httpContextAccessor,
    TokenContainer tokenContainer) : IAuthenticationService
{

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ChatbotApi");
    private readonly TokenContainer _tokenContainer = tokenContainer;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public bool IsAuthenticated { get; set; } = false;

    public CurrentUserDto CurrentUser { set; get; } = default!;



    public async Task<bool?> LoginAsync(string email, string password)
    {
        var request = CreateHttpRequest(new { Email = email, Password = password });
        var response = await _httpClient.PostAsync("identity/login", request);

        if (!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonConvert.DeserializeObject<TokenContainer>(content);
        if(tokenResponse== null) return null;
        _tokenContainer.SetRefreshToken(tokenResponse.RefreshToken!);
        _tokenContainer.SetAccessToken(tokenResponse.AccessToken);  
        _tokenContainer.SetExpiresIn(tokenResponse.ExpiresIn);
        return true;
    }

    public async Task<bool> RegisterAsync(RegisterDto model)
    {
        var request = CreateHttpRequest(new 
        { 
            model.Email,
            model.Password,
            model.FirstName,
            model.LastName 
        });
        var response = await _httpClient.PostAsync("identity/register", request);
        if (!response.IsSuccessStatusCode)
            return false;

        return true;

    }

    private static StringContent CreateHttpRequest(object data)
    {
        var json = JsonConvert.SerializeObject(data);

        return new StringContent(json,Encoding.UTF8,"application/json");

    }

    public Task LogoutAsync()
    {
        _tokenContainer.AccessToken = string.Empty;
        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available.");
        return context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task SignInWithTokenAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<CurrentUserDto>("account/me");
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
        var expiration = _tokenContainer.ExpiresIn;
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
        IsAuthenticated = true;
        CurrentUser = new(response.Id, response.Email, [.. response.Roles]);

    }
}
