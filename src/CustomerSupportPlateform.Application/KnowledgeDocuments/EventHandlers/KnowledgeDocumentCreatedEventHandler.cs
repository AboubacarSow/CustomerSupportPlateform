namespace CustomerSupportPlateform.Application.KnowledgeDocuments.EventHandlers;


public class KnowledgeDocumentCreatedEventHandler(List<IEmbeddingGenerator> embeddingGenerators,
    List<IContentExtractor> contentExtractors) : INotificationHandler<KnowledgeDocumentCreatedEvent>
{
    private readonly List<IEmbeddingGenerator> _embeddingGenerators = embeddingGenerators;
    private readonly List<IContentExtractor> _contentExtractors = contentExtractors;
    private readonly IContentChunker chunker;
    private readonly IApplicationDbContext dbContext;
    private readonly ITempStorageService tmpStorage;

    public Task Handle(KnowledgeDocumentCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Get the file from tmp
        // ingest : extracting -> cleaning -> chunking -> embedding -> persisting 
        
        var generator = _embeddingGenerators.Select(emb => emb.Environment == ModelsEnvironment.Developpement);
        throw new NotImplementedException();
    }
}