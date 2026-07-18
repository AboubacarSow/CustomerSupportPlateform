using Pgvector;

namespace CustomerSupportPlateform.Application.Interfaces;

public interface IEmbeddingGenerator
{
    Vector GenerateEmbedding(string chunck);
}
