
namespace CustomerSupportPlateform.Application.Interfaces;

public interface IContentExtractor
{
    IngestionDocumentFormat Format{get;}
    string ExtractContent(string tempPath);
}
