using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Text;

namespace CustomerSupportPlateform.Infrastructure.Ingestions.ContentExtractors;

internal class MarkDownExtractor : IContentExtractor
{
   public string Format => TextContentTypes.MD;

    public string ExtractContent(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Markdown file was not found: {filePath}", filePath);

        var markdown = File.ReadAllText(filePath, Encoding.UTF8);

        return Normalize(markdown);
    }

    private static string Normalize(string markdown)
    {
        return markdown
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Replace("\t", "    ")
            .Replace("\u00A0", " ")
            .Trim();
    }
}