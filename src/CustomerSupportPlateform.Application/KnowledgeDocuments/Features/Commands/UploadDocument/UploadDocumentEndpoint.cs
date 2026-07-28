using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Metadata;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.Features.Commands.UploadDocument;



public record UploadDocumentResponse(Guid Id,string Title,string? Description,IndexStatus IndexStatus);
public class UploadDocumentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/knowledges", Handle)
        .Produces<UploadDocumentResponse>((int)HttpStatusCode.OK)
        .WithDescription("Uploads Knowledge Document")
        .WithTags("Knowledges")
        .DisableAntiforgery();
    }

    private async Task<IResult> Handle ([FromServices] IMediator sender,
           [FromForm]string title,
           [FromForm]string description,
           IFormFile file)
    {
            var(document_id, document_title, document_description, document_status) = await sender.Send(new UploadDocumentCommand
                                                                        (title,
                                                                        description!,
                                                                        file));
            //return Results.CreatedAtRoute("/api/knowledges/", new { id });
            return Results.Ok(new UploadDocumentResponse(document_id, document_title, document_description, document_status));
    }
  
}