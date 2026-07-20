using Pgvector;

namespace CustomerSupportPlateform.Application.Interfaces;

public interface IVectorSearchSimilarity
{
    Task<List<string>> SearchAsync(Vector queryVector);
}
