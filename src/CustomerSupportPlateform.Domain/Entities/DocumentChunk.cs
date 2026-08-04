using CustomerSupportPlateform.Domain.Constants;
using Pgvector;

namespace CustomerSupportPlateform.Domain.Entities;

public class DocumentChunk : BaseEntity
{
    public int ChunkIndex { get; private set; }
    public Guid DocumentId { get; private set; }
    public KnowledgeDocument? Document { get; private set; }
    public string Chunk { get; private set; } = string.Empty;
    public Vector? Embedding { get; private set; }
    public Language Language {get;private set;}

    private DocumentChunk(int chunkIndex,Guid documentId,string chunk,Vector embedding, Language language)
    {
        ChunkIndex = chunkIndex;
        DocumentId = documentId;
        Chunk = chunk;
        Embedding = embedding;
        Language = language;
    }

    public static DocumentChunk CreateNew(int chunkIndex,Guid documentId,
                                    string chunk,Vector embedding,Language language)
                                    => new(chunkIndex,documentId,chunk,embedding, language);

}



