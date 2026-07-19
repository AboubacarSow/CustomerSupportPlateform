using Pgvector;

namespace CustomerSupportPlateform.Application.Interfaces;

public interface IEmbeddingGenerator
{
    Task<Vector> GenerateEmbedding(string chunk);
}
