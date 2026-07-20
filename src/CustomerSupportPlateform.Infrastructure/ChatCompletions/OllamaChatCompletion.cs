namespace CustomerSupportPlateform.Infrastructure.ChatCompletions;


internal class OllamaChatCompletion : IChatCompletion
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    public ModelsEnvironment Environment => ModelsEnvironment.Development;

    internal OllamaChatCompletion(IHttpClientFactory factory, IConfiguration configuration)
    {
        _client = factory.CreateClient("Ollama-Client");
        _configuration = configuration;
    }

    public async Task<string> RequestAsync(List<string> context, string question)
    {
        var systemContent = _configuration["PromptSettings:System"];

        var contextAsString = string.Join("\n", context);
        var systemContentWithContext = string.Join("\n",systemContent, $"[Factuel Context]:\n{contextAsString}");
        var request = new OllamaChatRequest(
            _configuration["Ollama:ChatModel"]!,
            false,
            [   new Message("system", systemContentWithContext!),
                new Message("user",question)
            ]
            );
        var response = await _client.PostAsJsonAsync("api/chat",request);
        response.EnsureSuccessStatusCode();
     
        var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>();
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
