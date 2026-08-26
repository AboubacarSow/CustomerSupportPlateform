using CustomerSupportPlateform.Application.Chats.ActionFilters;

namespace CustomerSupportPlateform.Application.Chats.Features.Commands.ChatCompletion;


public record ChatCompletionRequest(Guid SessionId,string Question);
public record ChatCompletionResponse(string Message);
public class ChatCompletionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chat", async (IMediator sender, ChatCompletionRequest request) =>
        {
            var result = await sender.Send(new ChatCompletionCommand(request.SessionId, request.Question));

            return Results.Ok(new ChatCompletionResponse(result.Message));
        }).WithTags("Chats");
        
    }
}