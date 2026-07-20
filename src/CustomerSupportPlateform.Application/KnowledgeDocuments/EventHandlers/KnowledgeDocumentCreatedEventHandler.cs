using CustomerSupportPlateform.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.EventHandlers;


public class KnowledgeDocumentCreatedEventHandler(List<IEmbeddingGenerator> embeddingGenerators,
    List<IContentExtractor> contentExtractors,IContentChunker chunker,
    IApplicationDbContext dbContext) : INotificationHandler<KnowledgeDocumentCreatedEvent>
{
    private readonly List<IEmbeddingGenerator> _embeddingGenerators = embeddingGenerators;
    private readonly List<IContentExtractor> _contentExtractors = contentExtractors;
    private readonly IContentChunker _chunker =chunker;
    private readonly IApplicationDbContext _dbContext = dbContext;

    public async Task Handle(KnowledgeDocumentCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Get KnowledgeDocument 
        var document = await _dbContext.KnowledgeDocuments.FirstOrDefaultAsync(d=>d.Id==notification.DocumentId,cancellationToken)
                        ??throw new KnowledgeDocumentNotFoundException($"KnowledgeDocument with Id: {notification.DocumentId} not found");
        //Mark As Indexing
        document.RaiseDomainEvent(new KnowledgeDocumentIndexingEvent(document.Id));
        // ingest : extracting -> cleaning -> chunking -> embedding -> persisting 
        var extractor = _contentExtractors.FirstOrDefault(e=>e.Format == notification.DocumentFormat);
        var generator = _embeddingGenerators.FirstOrDefault(emb => emb.Environment == ModelsEnvironment.Development);

        var content = extractor!.ExtractContent(notification.TmpfilePath);
        var chunks = _chunker.Chunk(content);
        
        var index = -1;
        foreach(var chunk in chunks)
        {
            var embeddedVector = await generator!.GenerateEmbeddingAsync(chunk);
            var documentChunk = DocumentChunk.CreateNew(index++,document.Id,chunk,embeddedVector);
            _dbContext.Add(documentChunk);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        throw new NotImplementedException();
    }
}

