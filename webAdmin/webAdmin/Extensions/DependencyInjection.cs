using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using webAdmin.Services;
using webAdmin.Utilities;

namespace webAdmin.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<Services.IAuthenticationService, AuthenticationManager>();
        services.AddScoped<IKnowledgeDocumentService, KnowledgeDocumentService>();
        services.AddScoped<IChatCompletionService, ChatCompletionService>();

        return services;

    }

    public static IServiceCollection ConfigureApiSettings(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddSingleton<TokenContainer>();

        services.AddScoped<TokenHandler>();

        services.AddHttpClient("ChatbotApi", options =>
        {
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            ArgumentException.ThrowIfNullOrEmpty(baseUrl);
            options.BaseAddress = new Uri(baseUrl);
            options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }).AddHttpMessageHandler<TokenHandler>();


        return services;
    }

    public static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                //options.AccessDeniedPath = "/account/accessDenied";

                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });
        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<HttpContextAccessor>();

        //services.AddScoped<HttpClient>();


    }
}