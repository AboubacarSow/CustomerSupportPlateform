using System.Text;
using DocumentFormat.OpenXml.Drawing;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace CustomerSupportPlateform.Infrastructure.Ingestions.ContentExtractors;

internal class PdfExtractor : IContentExtractor
{
    public string Format => TextContentTypes.PDF;

    public string ExtractContent(string tempPath)
    {
        if (!File.Exists(tempPath))
            throw new ArgumentNullException($"File with Path:{tempPath} does not exist in Temp localstrogefolder");
        using var pdfDocument = PdfDocument.Open(tempPath);
        var stringBuilder = new StringBuilder();
        var wordExtractor = NearestNeighbourWordExtractor.Instance;
        var pageSegmentor = DocstrumBoundingBoxes.Instance;
        var readingOrder = UnsupervisedReadingOrderDetector.Instance;
        foreach(var page in pdfDocument.GetPages())
        {
            var letters = page.Letters;
            var words = wordExtractor.GetWords(letters);
            var textBlocks = pageSegmentor.GetBlocks(words);
            var orderedTextBlocks = readingOrder.Get(textBlocks);

            foreach(var block in orderedTextBlocks)
            {
                stringBuilder.Append(block.Text);
            }
        }
        return stringBuilder.ToString();
    }
}


