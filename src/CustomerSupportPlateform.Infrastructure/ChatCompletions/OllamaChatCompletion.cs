using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json;

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
        var systemContent = _configuration["PromptSetting:System"];

        var contextAsString = string.Join("\"\\n\\n---\\n\\n\"\"\\n\\n---\\n\\n\"", context);
        var systemContentWithContext = string.Join("\"\\n\\n---\\n\\n\"", systemContent, $"[Factuel Context]:\n{contextAsString}");
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
                                        .Where(m => m.SessionId == sessionId)
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
            false,
            [..requestMessages]
            );
        var json = System.Text.Json.JsonSerializer.Serialize(
                    request,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

        _logger.LogInformation(json);
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
    [JsonPropertyName("think")] public bool Think { get; init; }

    //[JsonPropertyName("options")]
    //public OllamaOptions Options { get; init; } = new();

    public OllamaChatRequest(string model,bool stream,bool think, Message[] messages)
    {
        Model = model;
        Stream = stream;
        Think = think;  
        Messages = messages;
    }




}
public class OllamaOptions
{
    [JsonPropertyName("num_predict")]
    public int NumPredict { get; init; } = 300;

    [JsonPropertyName("temperature")]
    public double Temperature { get; init; } = 0.2;
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
        Role = role;
        Content = content;
    }
}
