
namespace CustomerSupportPlateform.Application.Interfaces;

public interface IContentExtractor
{
    string Format{get;}
    string ExtractContent(string tempPath);
}
