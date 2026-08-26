using Microsoft.Extensions.Logging;

namespace CustomerSupportPlateform.Application.Chats.ActionFilters;

public class ChatEndpointFilter(ILogger<ChatEndpointFilter> logger) : IEndpointFilter
{
    private readonly string _apiKeyHeaderName = "X-API-KEY";
    private readonly string secretapikey = "secret-key";

   private readonly ILogger<ChatEndpointFilter> _logger= logger;
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var expectedApiKey= context.HttpContext.Request.Headers[_apiKeyHeaderName];

        if(string.Compare(secretapikey, expectedApiKey, StringComparison.OrdinalIgnoreCase) != 0 )
        {
            _logger.LogWarning("API-KEY missing for this request");
            return Results.Unauthorized();
        }
        return await next(context);
        
    }

    
}