using CustomerSupportPlateform.Application.KnowledgeDocuments.Features.Dtos;
using CustomerSupportPlateform.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.Features.Queries.GetDocumentById;

public record GetDocumentByIdQuery(Guid Id):IRequest<DocumentDto>;

public class GetDocumentByIdHandler(IApplicationDbContext dbContext) :
IRequestHandler<GetDocumentByIdQuery, DocumentDto>
{
    public async ValueTask<DocumentDto> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await dbContext.KnowledgeDocuments
                            .FirstOrDefaultAsync(d=>d.Id== request.Id,
                            cancellationToken)
                    ?? throw new KnowledgeDocumentNotFoundException($"Knowlege document with Id:{request.Id} not found");
        

        return  DocumentMapper.ToDto(document);
    }
}


