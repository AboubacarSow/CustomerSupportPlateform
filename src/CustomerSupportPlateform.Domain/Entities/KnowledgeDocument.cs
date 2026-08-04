using CustomerSupportPlateform.Domain.Constants;
using CustomerSupportPlateform.Domain.DDD;
using CustomerSupportPlateform.Domain.Events;

namespace CustomerSupportPlateform.Domain.Entities;

public class KnowledgeDocument : BaseEntity, IHasDomainEvent
{
    public string Title { get; private set; }= default!;
    public string? Description { get; private set; }
    public string ContentType { get; private set; }= default!;
    public string OriginalFileName { get;private  set; } = default!;
    public string StoragePath { get; private set; } = default!;
    public long FileSize {get; private set;} = default!;
    public IndexStatus Status { get; private set; } = default!;
    public DateTime? IndexedAt { get; private set; } = default!;

    public Language Language {get;private set;}

    
    private KnowledgeDocument(string title, string? description, 
        string contentType, string originalFileName, 
        string storagePath, long fileSize,IndexStatus status, Language language)
    {
        Title = title;
        Description = description;
        ContentType = contentType;
        OriginalFileName = originalFileName;
        StoragePath = storagePath;
        FileSize = fileSize;
        Status = status;
        Language = language;
    }

    private KnowledgeDocument(string title,
      string contentType, string originalFileName,
      string storagePath, long fileSize, IndexStatus status,Language language)
    {
        Title = title;
        ContentType = contentType;
        OriginalFileName = originalFileName;
        StoragePath = storagePath;
        FileSize = fileSize;
        Status = status;
        Language = language;
    }

    public static KnowledgeDocument Create(string title, string? description,string fileName, 
        string contentType, string path,long size, Language  language)
    {

        KnowledgeDocument knowledgeDocument = !string.IsNullOrEmpty(description) ?
            new (title, description, contentType, fileName, path, size, IndexStatus.Pending, language) :
            new (title, contentType, fileName, path, size, IndexStatus.Pending, language);
        knowledgeDocument.RaiseDomainEvent(new KnowledgeDocumentCreatedEvent
            (knowledgeDocument.Id, knowledgeDocument.ContentType,knowledgeDocument.StoragePath));

        return knowledgeDocument;
    }
    
    public void MarkAsIndexed()
    {
        Status = IndexStatus.Indexed;
        IndexedAt = DateTime.UtcNow;
    }

    public void MarkAsIndexing()
    {
        Status = IndexStatus.Indexing;
    }

    #region Domain Driven Design
    private readonly List<IDomainEvent> _domains = [];
    [NotMapped]
    public IReadOnlyList<IDomainEvent> DomainEvents => _domains.AsReadOnly();
    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domains.Add(domainEvent);
    }
    public void ClearDomainEvents()
    {
        _domains.Clear();
    }
    #endregion 
}

public enum IndexStatus
{
    Pending,
    Indexing,
    Indexed,
    Failed
}

