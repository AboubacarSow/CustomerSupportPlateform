using Pgvector;

namespace CustomerSupportPlateform.Domain.Entities;

public class DocumentChunk : BaseEntity
{
    public int ChunkIndex { get; set; }
    public Guid DocumentId { get; set; }
    public KnowledgeDocument? Document { get; set; }
    public string Content { get; set; } = string.Empty;

    [Column(TypeName = "vector(1536)")]
    public Vector? Embedding { get; set; }

}



