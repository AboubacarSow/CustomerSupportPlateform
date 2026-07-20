

namespace CustomerSupportPlateform.Infrastructure.Embeddings;

internal class OllamaEmbeddingGenerator(IHttpClientFactory factory, 
    IConfiguration configuration) : IEmbeddingGenerator
{
    
    private readonly HttpClient _client = factory.CreateClient("Ollama-Client");
    private readonly IConfiguration _configuration = configuration;

    public ModelsEnvironment Environment => ModelsEnvironment.Development;

    public async Task<Vector> GenerateEmbeddingAsync(string chunk)
    {
        var request = new OllamaEmbedRequest
        (
            _configuration["Ollama:EmbeddingModel"]!,
            chunk
        );
        var response = await _client.PostAsJsonAsync("/api/embed", request); 
        response.EnsureSuccessStatusCode();         
        var result = await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>();
        var vector = new Vector(result!.Embedding);

        return vector;
    }
}

internal class OllamaEmbedResponse
{
    [JsonPropertyName("embedding")] internal float[] Embedding { get; init; }
    internal OllamaEmbedResponse(float[] embedding)
    {
        Embedding = embedding;
    }
}

internal class OllamaEmbedRequest
{
     [JsonPropertyName("model")] internal string Model { get; init; } = default!;
     [JsonPropertyName("input")] internal string Input { get; init; } = default!;

    internal OllamaEmbedRequest(string model, string input)
    {
        Model = model;
        Input = input;
    }
}
