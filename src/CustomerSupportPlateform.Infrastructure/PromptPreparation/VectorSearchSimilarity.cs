using CustomerSupportPlateform.Infrastructure.Persistence;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace CustomerSupportPlateform.Infrastructure.PromptPreparation;


internal class VectorSearchSimilarity : IVectorSearchSimilarity
{
    private readonly ApplicationDbContext _dbContext ;
    private readonly int _topK = 6;

    public VectorSearchSimilarity(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<string>> SearchAsync(Vector queryVector)
    {
        var result = await _dbContext.Chunks.OrderBy(c=>c.Embedding!.CosineDistance(queryVector))
                                            .Take(_topK)
                                            .ToListAsync();
        
        return [.. result.Select(c=>c.Chunk)];
    }
}

