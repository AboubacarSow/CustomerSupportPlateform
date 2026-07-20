using CustomerSupportPlateform.Infrastructure.Persistence;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace CustomerSupportPlateform.Infrastructure.PromptPreparation;


internal class VectorSearchSimilarity(ApplicationDbContext dbContext) : IVectorSearchSimilarity
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly int _topK = 6;
    public async Task<List<string>> SearchAsync(Vector queryVector)
    {
        var result = await _dbContext.Chunks.OrderBy(c=>c.Embedding!.CosineDistance(queryVector))
                                            .Take(_topK)
                                            .ToListAsync();
        
        return [.. result.Select(c=>c.Chunk)];
    }
}

