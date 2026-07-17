using Pgvector;

namespace CustomerSupportPlateform.Application.Interfaces;

public interface IEmbeddingService
{
    Vector GenerateEmbedding(string chunck);
}
