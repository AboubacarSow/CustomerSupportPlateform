using Pgvector;

namespace CustomerSupportPlateform.Domain.Entities;

public class DocumentChunk : BaseEntity
{
    public int ChunkIndex { get; set; }
    public Guid DocumentId { get; set; }
    public KnowledgeDocument? Document { get; set; }
    public string Chunk { get; set; } = string.Empty;
    public Vector? Embedding { get; set; }

    private DocumentChunk(int chunkIndex,Guid documentId,string chunk,Vector embedding)
    {
        ChunkIndex = chunkIndex;
        DocumentId = documentId;
        Chunk = chunk;
        Embedding = embedding;
    }

    public static DocumentChunk CreateNew(int chunkIndex,Guid documentId,
                                    string chunk,Vector embedding)
                                    => new(chunkIndex,documentId,chunk,embedding);

}



