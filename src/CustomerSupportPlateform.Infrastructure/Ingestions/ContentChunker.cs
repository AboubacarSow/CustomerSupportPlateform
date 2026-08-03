using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel.Text;

namespace CustomerSupportPlateform.Infrastructure.Ingestions;

internal class ContentChunker : IContentChunker
{
    private const int MaxTokens = 300;
    private const int Overlap = 30;

    public List<string> Chunk(string markdown, string contentType)
    {
        if (contentType != TextContentTypes.MD)
            return ChunkPlainText(markdown);

        return ChunkMarkdown(markdown);
    }

    private static List<string> ChunkMarkdown(string markdown)
    {
        var sections = SplitIntoSections(markdown);

        var chunks = new List<string>();

        foreach (var section in sections)
        {
            var lines = TextChunker.SplitMarkDownLines(
                section,
                MaxTokens);

            var paragraphs =
                TextChunker.SplitMarkdownParagraphs(
                    lines,
                    MaxTokens,
                    Overlap);

            chunks.AddRange(paragraphs);
        }

        return chunks;
    }

    private static List<string> ChunkPlainText(string text)
    {
        var lines = TextChunker.SplitPlainTextLines(
            text,
            MaxTokens);

        return TextChunker.SplitPlainTextParagraphs(
            lines,
            MaxTokens,
            Overlap);
    }

    private static List<string> SplitIntoSections(string markdown)
    {
        var sections = new List<string>();

        var builder = new StringBuilder();

        using var reader = new StringReader(markdown);

        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            if (Regex.IsMatch(line, @"^##\s"))
            {
                if (builder.Length > 0)
                {
                    sections.Add(builder.ToString().Trim());
                    builder.Clear();
                }
            }

            builder.AppendLine(line);
        }

        if (builder.Length > 0)
            sections.Add(builder.ToString().Trim());

        return sections;
    }
}
