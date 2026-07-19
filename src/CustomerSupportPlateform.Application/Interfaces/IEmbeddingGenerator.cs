using CustomerSupportPlateform.Domain.Constants;
using Pgvector;

namespace CustomerSupportPlateform.Application.Interfaces;

public interface IEmbeddingGenerator
{
    ModelsEnvironment Environment {get;}
    Task<Vector> GenerateEmbedding(string chunk);
}
