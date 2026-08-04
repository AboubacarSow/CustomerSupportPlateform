using CustomerSupportPlateform.Application.Chats.Features.Dtos;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

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
public class ChatCompletionHandler(IEnumerable<IChatCompletion> chatHandlers,IEmbeddingGenerator generator,
IVectorSearchSimilarity vectorSearch, ILogger<ChatCompletionHandler> logger) : IRequestHandler<ChatCompletionCommand, ChatResponseDto>
{
    public async ValueTask<ChatResponseDto> Handle(ChatCompletionCommand request, CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();

        var embedding = await generator.GenerateEmbeddingAsync(request.Question);
        logger.LogInformation("Embedding Elapsed:{Elapsed} ms",stopWatch.ElapsedMilliseconds);
        stopWatch.Restart();
        var context = await vectorSearch.SearchAsync(embedding,request.Question);
        logger.LogInformation("Vector search Elapsed:{Elapsed} ms", stopWatch.ElapsedMilliseconds);
        stopWatch.Restart();
        var ollamaChatHandler = chatHandlers.First(h => h.Environment == ModelsEnvironment.Development);
        
        var message = await ollamaChatHandler.RequestAsync(request.SessionId,context, request.Question);
        logger.LogInformation("Chat Response Elapsed:{Elapsed} ms", stopWatch.ElapsedMilliseconds);

        return new(message);
    }
}


