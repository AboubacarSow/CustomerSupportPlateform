using Pgvector;

namespace CustomerSupportPlateform.Application.Interfaces;

public interface IEmbeddingGenerator
{
    ModelsEnvironment Environment {get;}
    Task<Vector> GenerateEmbeddingAsync(string chunk);
}
