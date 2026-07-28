namespace CustomerSupportPlateform.Application.KnowledgeDocuments.Features.Dtos;

public class DocumentMapper
{
    public static DocumentDto ToDto(KnowledgeDocument document)=>
    new(document.Id,
        document.Title,
        document.Description,
        document.ContentType,
        document.OriginalFileName,
        document.FileSize,
        document.Status.ToString(),
        document.IndexedAt);
}
