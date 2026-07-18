using DocumentFormat.OpenXml.Packaging;

namespace CustomerSupportPlateform.Infrastructure.ContentExtractors;

internal class DocxExtractor : IContentExtractor
{
    public IngestionDocumentFormat Format => IngestionDocumentFormat.DOCX;
    public string ExtractContent(string tempPath)
    {
        if(File.Exists(tempPath))
            throw new ArgumentNullException($"File with Path:{tempPath} does not exist in Temp folder");
        using var wordProcessingDoc = WordprocessingDocument.Open(tempPath,false);
       
        var body = wordProcessingDoc.MainDocumentPart?.Document!.Body;
        return body!.InnerText;
    }
}
