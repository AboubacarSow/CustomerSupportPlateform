namespace CustomerSupportPlateform.Domain.Entities;

public class KnowledgeDocument : BaseEntity
{
    public string Title { get; set; }= default!;
    public string? Description { get; set; }
    public string ContentType { get; set; }= default!;
    public string OriginalFileName { get; set; } = default!;
    public string StoragePath { get; set; } = default!;
    public IndexStatus Status { get; set; } = default!;
    public DateTime? IndexedAt { get; set; } = default!;
}

public enum IndexStatus
{
    Pending,
    Indexing,
    Indexed,
    Failed
}

