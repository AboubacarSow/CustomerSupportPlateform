using CustomerSupportPlateform.Application.Chats.Dtos;

namespace CustomerSupportPlateform.Application.Chats.Commands.ChatCompletion;


public record ChatCompletionCommand(string Question): IRequest<Dtos.ChatResponseDto>;

public class ChatCompletionHandler(IChatCompletion chatHandler,IEmbeddingGenerator generator,
IVectorSearchSimilarity vectorSearch) : IRequestHandler<ChatCompletionCommand, ChatResponseDto>
{
    public async ValueTask<ChatResponseDto> Handle(ChatCompletionCommand request, CancellationToken cancellationToken)
    {
        var embedding = await generator.GenerateEmbeddingAsync(request.Question);
        var context = await vectorSearch.SearchAsync(embedding);
        var message = await chatHandler.RequestAsync(context, request.Question);
        return new(message);
    }
}
