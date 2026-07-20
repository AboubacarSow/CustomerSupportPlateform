namespace CustomerSupportPlateform.Application.Chats.Commands.ChatCompletion;


public record ChatCompletionRequest(string Question);
public record ChatCompletionResponse(string Message);
public class ChatCompletionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chat", async (IMediator sender, ChatCompletionRequest request) =>
        {
            var result = await sender.Send(new ChatCompletionCommand(request.Question));

            return Results.Ok(new ChatCompletionResponse(result.Message));
        });
    }
}