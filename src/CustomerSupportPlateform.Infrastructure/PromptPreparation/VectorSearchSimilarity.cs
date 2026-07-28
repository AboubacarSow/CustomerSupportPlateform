using Amazon.Runtime.Internal.Util;
using CustomerSupportPlateform.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Net.WebSockets;

namespace CustomerSupportPlateform.Infrastructure.PromptPreparation;


internal class VectorSearchSimilarity : IVectorSearchSimilarity
{
    private readonly ApplicationDbContext _dbContext ;
    private readonly int _topK = 3;
    private readonly ILogger<VectorSearchSimilarity> _logger;

    public VectorSearchSimilarity(ApplicationDbContext dbContext,ILogger<VectorSearchSimilarity> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<List<string>> SearchAsync(Vector queryVector)
    {
        var result = await _dbContext.Chunks.OrderBy(c=>c.Embedding!.CosineDistance(queryVector))
                                            .Take(_topK)
                                            .ToListAsync();
        var index = 0;
       foreach(var chunk in result)
        {
            _logger.LogInformation("Chunk {index}: {Chunk}", index++, chunk.Chunk);
        }
        
        return [.. result.Select(c=>c.Chunk)];
    }
}

