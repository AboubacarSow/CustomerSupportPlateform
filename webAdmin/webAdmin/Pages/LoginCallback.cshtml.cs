using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webAdmin.Services;

namespace webAdmin.Pages;



[AllowAnonymous]
public class LoginCallbackModel(IAuthenticationService authManager,
   ILogger<LoginCallbackModel> logger) : PageModel
{
    private readonly ILogger<LoginCallbackModel> _logger = logger;
    public string? ReturnUrl { get; set; }
    [FromQuery]
    public bool? IsTokenExist { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            if (IsTokenExist.HasValue && IsTokenExist.Value)
            {

                ReturnUrl = Url.Content("/");
                await authManager.SignInWithTokenAsync();
                return LocalRedirect("/");
            }
            else return Redirect("/auth/login");

        }
        catch (Exception ex)
        {
            _logger.LogError("@{Message}",ex.Message);
            return Redirect("/auth/login");
        }
    }
}
