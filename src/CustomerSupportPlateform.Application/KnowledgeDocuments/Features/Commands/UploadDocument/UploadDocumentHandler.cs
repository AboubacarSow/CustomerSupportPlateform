using CustomerSupportPlateform.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.Features.Commands.UploadDocument;

public record UploadDocumentCommand(string Title, string Description,Language Language, IFormFile File) : 
    IRequest<(Guid,string,string,IndexStatus)>;

public class UploadDocumentValidator: AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentValidator()
    {
        RuleFor(d=>d.Title).NotEmpty()
            .WithMessage("Title Field cannot be Empty");

        RuleFor(d => d.File).NotNull()
            .WithMessage("File is required");

        RuleFor(d=>d.Language).NotEmpty()
            .WithMessage("Language is required");
    }
}
public class UploadDocumentHandler(ILocalStorage localStorage,
    IApplicationDbContext dbContext) : IRequestHandler<UploadDocumentCommand, (Guid, string, string, IndexStatus)>
{
    public async ValueTask<(Guid, string, string, IndexStatus)> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await request.File.CopyToAsync(stream, cancellationToken);
        

        var isFileTheSameNameExist = localStorage.IsFileAlreadyExists(request.File.FileName,request.Language);

        var localStoragePath = await localStorage.UploadFileToTempAsync(request.File,request.Language);
        

        if (isFileTheSameNameExist)
        {
            var document = await dbContext.KnowledgeDocuments.FirstOrDefaultAsync(x=>x.StoragePath == localStoragePath,
                cancellationToken)
               ?? throw new KnowledgeDocumentNotFoundException($"KnowledgeDocument with local storage:{localStoragePath} not found");


            document.UpgradeDocumentContent(request.Title,
                request.Description,
                request.File.Length);

            document.RaiseDomainEvent(new KnowledgeDocumentContentUpgradedEvent(document.Id,
                                document.ContentType,
                                document.StoragePath));

            await dbContext.SaveChangesAsync(cancellationToken);

            return (document.Id,
               document.Title,
               document.Description!,
               document.Status);

        }
        else
        {
            var knowledgeDocument = KnowledgeDocument.Create(request.Title, request.Description,
                                request.File.FileName,
                                request.File.ContentType,
                                localStoragePath, request.File.Length,
                                request.Language);

             dbContext.Add(knowledgeDocument);

            await dbContext.SaveChangesAsync(cancellationToken);

            return (knowledgeDocument.Id,
           knowledgeDocument.Title,
           knowledgeDocument.Description!,
           knowledgeDocument.Status);
        }
         

       

    }

  
}
