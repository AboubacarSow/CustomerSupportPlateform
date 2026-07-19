


namespace CustomerSupportPlateform.Application.KnowledgeDocuments.Commands.UploadDocument;

public record UploadDocumentRequest(string Title,string Description, IFormFile File);
public record UploadDocumentResponse(Guid Id,string Title,string? Description,IndexStatus IndexStatus);
public class UploadDocumentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/knowledges", async (IMediator sender,UploadDocumentRequest request) =>
        {
            var (id,title,description,status) = await sender.Send(new UploadDocumentCommand
                                                                        (request.Title,
                                                                        request.Description,
                                                                        request.File));
            //return Results.CreatedAtRoute("/api/knowledges/", new { id });
            return Results.Ok(new UploadDocumentResponse(id, title, description, status));
        }).Produces<UploadDocumentResponse>((int)HttpStatusCode.OK)
        .WithDescription("Uploads Knowledge Document");
    }
}