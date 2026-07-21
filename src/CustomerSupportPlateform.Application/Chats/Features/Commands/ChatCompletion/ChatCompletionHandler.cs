using CustomerSupportPlateform.Application.Chats.Features.Dtos;

namespace CustomerSupportPlateform.Application.Chats.Features.Commands.ChatCompletion;


public record ChatCompletionCommand(Guid SessionId,string Question): IRequest<Dtos.ChatResponseDto>;

public class ChatCompletionCommandValidator: AbstractValidator<ChatCompletionCommand>
{
    public ChatCompletionCommandValidator(){
        RuleFor(c => c.SessionId)
            .NotNull()
            .NotEmpty()
            .WithMessage("SessionId is required");
        RuleFor(c => c.Question).NotEmpty()
            .WithMessage("Question cannot be empty");
    }
}
public class ChatCompletionHandler(IChatCompletion chatHandler,IEmbeddingGenerator generator,
IVectorSearchSimilarity vectorSearch) : IRequestHandler<ChatCompletionCommand, ChatResponseDto>
{
    public async ValueTask<ChatResponseDto> Handle(ChatCompletionCommand request, CancellationToken cancellationToken)
    {
        var embedding = await generator.GenerateEmbeddingAsync(request.Question);
        var context = await vectorSearch.SearchAsync(embedding);
        var message = await chatHandler.RequestAsync(request.SessionId,context, request.Question);
        return new(message);
    }
}
