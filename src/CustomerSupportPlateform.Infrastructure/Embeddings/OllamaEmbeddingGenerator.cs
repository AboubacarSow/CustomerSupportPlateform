

using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CustomerSupportPlateform.Infrastructure.Embeddings;

internal class OllamaEmbeddingGenerator : IEmbeddingGenerator
{
    
    private readonly HttpClient _client ;
    private readonly IConfiguration _configuration ;
    private readonly ILogger<OllamaEmbeddingGenerator> _logger ;

    public ModelsEnvironment Environment => ModelsEnvironment.Development;

    public OllamaEmbeddingGenerator(IHttpClientFactory factory,ILogger<OllamaEmbeddingGenerator> logger,
    IConfiguration configuration)
    {
        _client = factory.CreateClient("Ollama-Client");
        _configuration = configuration;
        _logger = logger;
    }
    public async Task<Vector> GenerateEmbeddingAsync(string chunk)
    {
        var request = new OllamaEmbedRequest
        (
            _configuration["Ollama:EmbeddingModel"],
            chunk
        );
       
        var response = await _client.PostAsJsonAsync("/api/embed", request);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Embedding request Failed:{Message}", body);
            throw new HttpRequestException("Failed to embed chunk");
        }
        //response.EnsureSuccessStatusCode();         
        var result = await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>();
        var vector = new Vector(result!.Embedding[0]);


        return vector;
    }
}

public class OllamaEmbedResponse
{
    [property: JsonPropertyName("model")]
    string Model { get; set;  }
    [JsonPropertyName("embeddings")] public float[][] Embedding { get; init; }
    public OllamaEmbedResponse(string model,float[][] embedding)
    {
        Model = model;
        Embedding = embedding;
    }
    public OllamaEmbedResponse() { }
}

public class OllamaEmbedRequest
{
     [JsonPropertyName("model")] public string Model { get; init; } = default!;
     [JsonPropertyName("input")] public string Input { get; init; } = default!;

    public OllamaEmbedRequest(string model, string input)
    {
        Model = model;
        Input = input;
    }
}
