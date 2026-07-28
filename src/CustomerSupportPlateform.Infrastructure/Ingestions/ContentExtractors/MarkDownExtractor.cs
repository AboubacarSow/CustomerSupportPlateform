using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Text;

namespace CustomerSupportPlateform.Infrastructure.Ingestions.ContentExtractors;

internal class MarkDownExtractor : IContentExtractor
{
    public string Format => TextContentTypes.MD;
    public string ExtractContent(string tempPath)
    {
        if(!File.Exists(tempPath))
            throw new ArgumentNullException($"File with Path:{tempPath} does not exist in Temp localstrogefolder");

        var stringBuilder = new StringBuilder();
        var markdow = File.ReadAllText(tempPath);

        var document = Markdown.Parse(markdow);

        var allParagraphs = document.Descendants<ParagraphBlock>().ToArray();

        foreach( var block in allParagraphs)
        {
            if (block.Inline == null) continue;
            foreach (var inline in block.Inline.Descendants<LiteralInline>())
            {
                stringBuilder.Append(inline.Content);
            }
                
        }
        

        return stringBuilder.ToString();

    }
}