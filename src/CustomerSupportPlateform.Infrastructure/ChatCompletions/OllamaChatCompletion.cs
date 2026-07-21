namespace CustomerSupportPlateform.Infrastructure.ChatCompletions;


internal class OllamaChatCompletion : IChatCompletion
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    public ModelsEnvironment Environment => ModelsEnvironment.Development;
    private readonly IApplicationDbContext _dbContext;

    public OllamaChatCompletion(IHttpClientFactory factory, IConfiguration configuration,IApplicationDbContext dbContext)
    {
        _client = factory.CreateClient("Ollama-Client");
        _configuration = configuration;
        _dbContext = dbContext;
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
        response.EnsureSuccessStatusCode();
     
        var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>();
        var newConversation = ConversationMessage.CreateNew(sessionId,"assistant",result!.Message);
        _dbContext.Add(newConversation);
        session?.SetLastUpdatedAt();

        await _dbContext.SaveChangesAsync();
        return result!.Message;
    }
}

internal class OllamaChatRequest
{
    [JsonPropertyName("model")]internal string Model { get; init;}
    [JsonPropertyName("stream")] internal bool Stream { get; init;}
    [JsonPropertyName("messages")] internal Message[] Messages { get; init;}

    internal OllamaChatRequest(string model,bool stream, Message[] messages)
    {
        Model = model;
        Stream = stream;
        Messages = messages;
    }



}
internal class OllamaChatResponse
{
    [JsonPropertyName("message")] internal string Message { get; init;}
    internal OllamaChatResponse(string message)
    {
        Message = message;
    }
}

internal class Message
{
    [JsonPropertyName("role")] internal string Role { get; init;}
    [JsonPropertyName("content")] internal string Content { get; init;}

    internal Message(string role, string content)
    {
        Role = role; Content = content;
    }
}
