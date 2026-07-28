using CustomerSupportPlateform.Application.KnowledgeDocuments.Features.Dtos;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.Features.Queries.GetDocumentById;

public class GetDocumentByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/knowledges/{id:Guid}", async (IMediator sender,Guid id) =>
        {
            var result = await sender.Send(new GetDocumentByIdQuery(id));

            return Results.Ok(result);
        }).Produces<DocumentDto>((int)HttpStatusCode.OK)
        .WithDescription("Gets Knowledge Document Details")
        .WithTags("Knowledges");
    }
}


