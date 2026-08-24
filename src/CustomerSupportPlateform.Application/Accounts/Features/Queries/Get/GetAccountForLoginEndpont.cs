using System.Security.Claims;

namespace CustomerSupportPlateform.Application.Accounts.Features.Queries.Get;

public class GetAccountForLoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
       app.MapGet("/api/account/me", (ClaimsPrincipal user) =>
        {
            return Results.Ok(new
            {
                Id = user.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = user.FindFirstValue(ClaimTypes.Email),
                Roles = user.FindAll(ClaimTypes.Role)
                            .Select(r => r.Value)
            });
        })
         .WithTags("Identity")
        .RequireAuthorization();
    }
}