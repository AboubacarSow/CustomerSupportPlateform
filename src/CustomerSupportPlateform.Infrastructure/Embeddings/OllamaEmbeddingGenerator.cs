using Microsoft.Extensions.Configuration;
using Pgvector;
using System.Net.Http.Json;

namespace CustomerSupportPlateform.Infrastructure.Embeddings;

internal class OllamaEmbeddingGenerator(IHttpClientFactory factory, 
    IConfiguration configuration) : IEmbeddingGenerator
{
    private readonly HttpClient _client = factory.CreateClient("Ollama-Client");
    private readonly IConfiguration _configuration = configuration;

    public async Task<Vector> GenerateEmbedding(string chunk)
    {
        var request = new
        {
            model = _configuration["Ollama:EmbeddingModel"],
            input = chunk
        };
        var result = await _client.PostAsJsonAsync("/api/embed", request);
        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync();
            throw new HttpRequestException(error);
        } 
        var response = await result.Content.ReadFromJsonAsync<OllamaEmbedResponse>();
        var vector = new Vector(response!.Embedding);

        return vector;
    }
}

internal record OllamaEmbedResponse(float[] Embedding);
internal record OllamaEmbedRequest(string Model, string Prompt);