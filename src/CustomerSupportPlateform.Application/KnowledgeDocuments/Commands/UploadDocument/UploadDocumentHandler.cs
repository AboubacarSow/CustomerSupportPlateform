
using Microsoft.AspNetCore.Http;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.Commands.UploadDocument;

public record UploadDocumentCommand(string Title, string Description, IFormFile File) : 
    IRequest<(Guid,string,string,IndexStatus)>;

public class UploadDocumentValidator: AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentValidator()
    {
        RuleFor(d=>d.Title).NotEmpty()
            .WithMessage("Title Field cannot be Empty");

        RuleFor(d => d.File).NotNull()
            .WithMessage("File is required");
    }
}
public class UploadDocumentHandler(IBlobStorage railwayStorageService,IApplicationDbContext dbContext) : IRequestHandler<UploadDocumentCommand, (Guid, string, string, IndexStatus)>
{
    public async ValueTask<(Guid, string, string, IndexStatus)> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await request.File.CopyToAsync(stream, cancellationToken);
        
      
        var (key,size,contentType) = await railwayStorageService.UploadAsync(stream,
                                    request.File.FileName,
                                    request.File.ContentType,cancellationToken);

        var knowledgeDocument = KnowledgeDocument.Create(request.Title, request.Description,
            request.File.FileName, contentType, key, size);

         dbContext.Add(knowledgeDocument);
         await dbContext.SaveChangesAsync(cancellationToken);

        return (knowledgeDocument.Id,
            knowledgeDocument.Title,
            knowledgeDocument.Description! ,
            knowledgeDocument.Status);

    }
}
