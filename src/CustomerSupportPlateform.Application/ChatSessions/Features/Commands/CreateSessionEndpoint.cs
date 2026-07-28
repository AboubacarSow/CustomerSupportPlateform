namespace CustomerSupportPlateform.Application.ChatSessions.Features.Commands;

public record CreateSessionRequest(Guid Id);
public class CreateSessionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/sessions", async (IMediator sender, CreateSessionRequest request) =>
        {
            var newSession = await sender.Send(new CreateSessionCommand(request.Id));

            return Results.Ok(newSession);
        }).WithDisplayName("Create a session to start chating")
           .WithTags("Sessions");
    }
}