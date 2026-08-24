using CustomerSupportPlateform.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.EventHandlers;


public class KnowledgeDocumentCreatedEventHandler(IEnumerable<IEmbeddingGenerator> embeddingGenerators,
    IEnumerable<IContentExtractor> contentExtractors,IContentChunker chunker,ILogger<KnowledgeDocumentCreatedEventHandler> logger,
    IApplicationDbContext dbContext) : INotificationHandler<KnowledgeDocumentCreatedEvent>
{
    private readonly IEnumerable<IEmbeddingGenerator> _embeddingGenerators = embeddingGenerators;
    private readonly IEnumerable<IContentExtractor> _contentExtractors = contentExtractors;
    private readonly IContentChunker _chunker =chunker;
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<KnowledgeDocumentCreatedEventHandler> _logger = logger;

    public async Task Handle(KnowledgeDocumentCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Get KnowledgeDocument 
        var document = await _dbContext.KnowledgeDocuments.FirstOrDefaultAsync(d=>d.Id==notification.DocumentId,cancellationToken)
                        ??throw new KnowledgeDocumentNotFoundException($"KnowledgeDocument with Id: {notification.DocumentId} not found");
        //Mark As Indexing
        document.RaiseDomainEvent(new KnowledgeDocumentIndexingEvent(document.Id));

        _logger.LogInformation("Start indexing document with ID:{DocumentId}",document.Id);

        // ingest : extracting -> cleaning -> chunking -> embedding -> persisting 
        var extractor = _contentExtractors.FirstOrDefault(e=>e.Format == notification.ContentType);
        var generator = _embeddingGenerators.FirstOrDefault(emb => emb.Environment == ModelsEnvironment.Development);

        var content = extractor!.ExtractContent(notification.LocalPath);
        var chunks = _chunker.Chunk(content,notification.ContentType);
        
        var index = 0;
        foreach(var chunk in chunks)
        {
            var embeddedVector = await generator!.GenerateEmbeddingAsync(chunk);
            if (embeddedVector is null)
            {
                // either all chunks get embeded or either way document get failed as IndexStatus
                document.RaiseDomainEvent(new KnowledgeDocumentFailedIndexingEvent(document.Id));
                _logger.LogWarning("Failed indexing document with Id:{DocumentId}",document.Id);
                return ;
                
            }
            var documentChunk = DocumentChunk.CreateNew(index++,document.Id,chunk,embeddedVector,document.Language);
            _dbContext.Add(documentChunk);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

 
}


public class KnowledgeDocumentContentUpgradedEventHandler(IEnumerable<IEmbeddingGenerator> embeddingGenerators,
    IEnumerable<IContentExtractor> contentExtractors, IContentChunker chunker, ILogger<KnowledgeDocumentContentUpgradedEventHandler> logger,
    IApplicationDbContext dbContext) : INotificationHandler<KnowledgeDocumentContentUpgradedEvent>
{
    private readonly IEnumerable<IEmbeddingGenerator> _embeddingGenerators = embeddingGenerators;
    private readonly IEnumerable<IContentExtractor> _contentExtractors = contentExtractors;
    private readonly IContentChunker _chunker = chunker;
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<KnowledgeDocumentContentUpgradedEventHandler> _logger = logger;

    public async Task Handle(KnowledgeDocumentContentUpgradedEvent notification, CancellationToken cancellationToken)
    {
        // Get KnowledgeDocument 
        var document = await _dbContext.KnowledgeDocuments.FirstOrDefaultAsync(d => d.Id == notification.DocumentId, cancellationToken)
                        ?? throw new KnowledgeDocumentNotFoundException($"KnowledgeDocument with Id: {notification.DocumentId} not found");
        //Mark As Indexing
        document.RaiseDomainEvent(new KnowledgeDocumentIndexingEvent(document.Id));

        _logger.LogInformation("Start indexing document with ID:{DocumentId}", document.Id);

        // ingest : extracting -> cleaning -> chunking -> embedding -> persisting 
        var extractor = _contentExtractors.FirstOrDefault(e => e.Format == notification.ContentType);
        var generator = _embeddingGenerators.FirstOrDefault(emb => emb.Environment == ModelsEnvironment.Development);

        var content = extractor!.ExtractContent(notification.LocalPath);
        var chunks = _chunker.Chunk(content, notification.ContentType);

        var oldChunks  = await _dbContext.Chunks.Where(x=>x.DocumentId == document.Id)
                        .ToListAsync(cancellationToken);

        _dbContext.RemoveRange(oldChunks);

        var index = 0;
        foreach (var chunk in chunks)
        {
            var embeddedVector = await generator!.GenerateEmbeddingAsync(chunk);
            if (embeddedVector is null)
            {
                // either all chunks get embeded or either way document get failed as IndexStatus
                document.RaiseDomainEvent(new KnowledgeDocumentFailedIndexingEvent(document.Id));
                _logger.LogWarning("Failed indexing document with Id:{DocumentId}", document.Id);
                return;

            }
            var documentChunk = DocumentChunk.CreateNew(index++, document.Id, chunk, embeddedVector, document.Language);
            _dbContext.Add(documentChunk);
        }

        document.MarkAsIndexed();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }


}
