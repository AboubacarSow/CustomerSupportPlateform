using Markdig;
using Markdig.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace CustomerSupportPlateform.Infrastructure.ContentExtractors;

internal class MarkDownExtractor : IContentExtractor
{
    public IngestionDocumentFormat Format => IngestionDocumentFormat.MD;
    public string ExtractContent(string tempPath)
    {
        if(File.Exists(tempPath))
            throw new ArgumentNullException($"File with Path:{tempPath} does not exist in Temp folder");

        var stringBuilder = new StringBuilder();
        var markdow = File.ReadAllText(tempPath);

        var document = Markdown.Parse(markdow);

        var allParagraphs = document.Descendants<ParagraphBlock>().ToArray();

        var content = allParagraphs.Select(x => x.Inline!.FirstChild)
            .Cast<Markdig.Syntax.Inlines.LiteralInline>()
            .Select(x => x.Content).ToArray();

        foreach (var slice in content)
            stringBuilder.Append(slice.Text);

        return stringBuilder.ToString();

    }
}