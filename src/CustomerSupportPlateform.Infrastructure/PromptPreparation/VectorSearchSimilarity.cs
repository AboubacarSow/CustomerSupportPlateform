using Amazon.Runtime.Internal.Util;
using CustomerSupportPlateform.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Net.WebSockets;

namespace CustomerSupportPlateform.Infrastructure.PromptPreparation;


internal class VectorSearchSimilarity(ApplicationDbContext context) : IVectorSearchSimilarity
{
    private readonly ApplicationDbContext _dbContext = context;
    private readonly int _topK = 4;

   
    public async Task<List<string>> SearchAsync(Vector queryVector,string queryString)
    {
        var language = LanguageDetector.Detect(queryString);
        var result = await _dbContext.Chunks
            .Where(c=>c.Language == language)
            .Select(c => new { c.Chunk, Distance = c.Embedding!.CosineDistance(queryVector) })
            .OrderBy(c => c.Distance)
            .Take(_topK)
            .ToListAsync();

        return [.. result.Select(c => c.Chunk)];
    }
}

