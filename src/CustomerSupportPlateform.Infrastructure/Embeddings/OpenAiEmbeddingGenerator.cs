using Pgvector;

namespace CustomerSupportPlateform.Infrastructure.Embeddings;

internal class OpenAiEmbeddingGenerator : IEmbeddingGenerator
{
    public Task<Vector> GenerateEmbedding(string chunck)
    {
        throw new NotImplementedException();
    }
}
