namespace CustomerSupportPlateform.Infrastructure.Embeddings;

internal class OpenAiEmbeddingGenerator : IEmbeddingGenerator
{
    public ModelsEnvironment Environment => ModelsEnvironment.Production;

    public Task<Vector?> GenerateEmbeddingAsync(string chunck)
    {
        throw new NotImplementedException();
    }
}
