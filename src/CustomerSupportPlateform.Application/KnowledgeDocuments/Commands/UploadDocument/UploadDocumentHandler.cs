using ConduitR.Abstractions;
using CustomerSupportPlateform.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.Commands.UploadDocument;

public record UploadDocumentCommand(IFormFile File, string Title, string Description): IRequest<Guid>;

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
public class UploadDocumentHandler(IBlobStorage railwayStorageService,IApplicationDbContext dbContext) : IRequestHandler<UploadDocumentCommand, Guid>
{
    public async ValueTask<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await request.File.CopyToAsync(stream, cancellationToken);

        
      
        var (key,size,contentType) = await railwayStorageService.UploadAsync(stream,
                                    request.File.FileName,
                                    request.File.ContentType,cancellationToken);

        var knowledgeDocument = KnowledgeDocument.Create(request.Title, request.Description,
            request.File.FileName, contentType, key, size);

         dbContext.Add<KnowledgeDocument>(knowledgeDocument);
         await dbContext.SaveChangesAsync(cancellationToken);

        return knowledgeDocument.Id;

    }
}