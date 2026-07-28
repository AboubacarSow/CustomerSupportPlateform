using DocumentFormat.OpenXml.Packaging;
using System.Text;

namespace CustomerSupportPlateform.Infrastructure.Ingestions.ContentExtractors;

internal class DocxExtractor : IContentExtractor
{
    public string Format => TextContentTypes.DOCX;
    public string ExtractContent(string tempPath)
    {
        //var stringBuilder = new StringBuilder();
        if(!File.Exists(tempPath))
            throw new ArgumentNullException($"File with Path:{tempPath} does not exist in Temp localstrogefolder");
        using var wordProcessingDoc = WordprocessingDocument.Open(tempPath,false);
       
        var body = wordProcessingDoc.MainDocumentPart?.Document!.Body;

        //foreach (var paragraph in body!.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
        //{
        //    foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
        //    {
        //        stringBuilder.Append(text.Text);
        //    }
        //    stringBuilder.Append(' ');
        //}
        //return stringBuilder.ToString();

        return body!.InnerText;
    }
}
