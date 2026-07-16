using Pgvector;

namespace CustomerSupportPlateform.Domain.Entities;

public class DocumentChunk : BaseEntity
{
    public int ChunkIndex { get; set; }
    public Guid DocumentId { get; set; }
    public KnowledgeDocument? Document { get; set; }
    public string Content { get; set; } = string.Empty;
    public Vector? Embedding { get; set; }

}



