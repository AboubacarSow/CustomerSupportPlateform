using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CustomerSupportPlateform.Infrastructure.ChatCompletions;


internal class OllamaChatCompletion : IChatCompletion
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    public ModelsEnvironment Environment => ModelsEnvironment.Development;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<OllamaChatCompletion> _logger;

    public OllamaChatCompletion(IHttpClientFactory factory,ILogger<OllamaChatCompletion> logger,
        IConfiguration configuration,IApplicationDbContext dbContext)
    {
        _client = factory.CreateClient("Ollama-Client");
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<string> RequestAsync(Guid sessionId, List<string> context, string question)
    {
        var systemContent = _configuration["PromptSettings:System"];

        var contextAsString = string.Join("\n", context);
        var systemContentWithContext = string.Join("\n",systemContent, $"[Factuel Context]:\n{contextAsString}");
        var session =await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        List<Message> usermessages =[];
        List<Message> assistantMessages=[];
        if (session is null)
        {
            session = Session.CreateNew(sessionId);
            _dbContext.Add(session);
        }
        else
        {
            var messages = _dbContext.ConversationMessages
                                        .AsNoTracking()
                                        .Where(m => m.Id == sessionId)
                                        .OrderBy(c=>c.CreatedAt);

            usermessages = [.. messages.Where(m => m.Role == "user")
                .Select(m => new Message(m.Role, m.Content))];
            assistantMessages = [.. messages.Where(m => m.Role == "assisant")
                .Select(m=> new Message(m.Role,m.Content))];
        }
        usermessages.Add(new("user", question));
        var requestMessages = new List<Message>()
        {
             new("system",systemContentWithContext!),
        };
        usermessages.ForEach(requestMessages.Add);
        if(assistantMessages.Count > 0) 
            assistantMessages.ForEach(requestMessages.Add);

        var request = new OllamaChatRequest(
            _configuration["Ollama:ChatModel"]!,
            false,
            [..requestMessages]
            );
        var response = await _client.PostAsJsonAsync("api/chat",request);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("ChatCompletion request Failed:{Message}", body);
            throw new HttpRequestException("Failed to chat");
        }
        response.EnsureSuccessStatusCode();
     
        var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>();
        var newConversation = ConversationMessage.CreateNew(sessionId,"assistant",result!.Message.Content);
        _dbContext.Add(newConversation);
        session?.SetLastUpdatedAt();

        await _dbContext.SaveChangesAsync();
        return result!.Message.Content;
    }
}

public class OllamaChatRequest
{
    [JsonPropertyName("model")]public string Model { get; init;}
    [JsonPropertyName("stream")] public bool Stream { get; init;}
    [JsonPropertyName("messages")] public Message[] Messages { get; init;}

    public OllamaChatRequest(string model,bool stream, Message[] messages)
    {
        Model = model;
        Stream = stream;
        Messages = messages;
    }



}
public class OllamaChatResponse
{
    [JsonPropertyName("message")] public Message Message { get; init;}
    [JsonPropertyName("model")] public string Model { get; init;}
   
    public OllamaChatResponse(string model,Message message)
    {
        Model = model;
        Message = message;
    }
}

public class Message
{
    [JsonPropertyName("role")] public string Role { get; init;}
    [JsonPropertyName("content")] public string Content { get; init;}

    public Message(string role, string content)
    {
        Role = role; Content = content;
    }
}
