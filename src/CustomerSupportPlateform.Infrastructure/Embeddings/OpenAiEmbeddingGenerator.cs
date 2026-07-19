using Pgvector;

namespace CustomerSupportPlateform.Infrastructure.Embeddings;

internal class OpenAiEmbeddingGenerator : IEmbeddingGenerator
{
    public ModelsEnvironment Environment => ModelsEnvironment.Production;

    public Task<Vector> GenerateEmbedding(string chunck)
    {
        throw new NotImplementedException();
    }
}
