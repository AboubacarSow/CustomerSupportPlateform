using CustomerSupportPlateform.Domain.DDD;

namespace CustomerSupportPlateform.Domain.Entities;

public class KnowledgeDocument : BaseEntity, IHasDomainEvent
{
    public string Title { get; set; }= default!;
    public string? Description { get; set; }
    public string ContentType { get; set; }= default!;
    public string OriginalFileName { get; set; } = default!;
    public string StoragePath { get; set; } = default!;
    public long FileSize {get; set;} = default!;
    public IndexStatus Status { get; set; } = default!;
    public DateTime? IndexedAt { get; set; } = default!;

    
    private KnowledgeDocument(string title, string? description, 
        string contentType, string originalFileName, 
        string storagePath, long fileSize,IndexStatus status)
    {
        Title = title;
        Description = description;
        ContentType = contentType;
        OriginalFileName = originalFileName;
        StoragePath = storagePath;
        FileSize = fileSize;
        Status = status;
    }

    private KnowledgeDocument(string title,
      string contentType, string originalFileName,
      string storagePath, long fileSize, IndexStatus status)
    {
        Title = title;
        ContentType = contentType;
        OriginalFileName = originalFileName;
        StoragePath = storagePath;
        FileSize = fileSize;
        Status = status;
    }

    public static KnowledgeDocument Create(string title, string? description,string fileName, 
        string contentType, string path,long size)
    {
        return !string.IsNullOrEmpty(description) ?
            new (title, description, contentType, fileName, path, size, IndexStatus.Pending) :
            new (title, contentType, fileName, path, size, IndexStatus.Pending);
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

