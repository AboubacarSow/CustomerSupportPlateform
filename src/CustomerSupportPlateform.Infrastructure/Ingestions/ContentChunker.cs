using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel.Text;

namespace CustomerSupportPlateform.Infrastructure.Ingestions;

internal sealed partial class ContentChunker : IContentChunker
{
    private const int MaxTokens = 300;
    private const int Overlap = 30;
    private const int MinSectionWords = 12;

    public List<string> Chunk(string markdown, string contentType)
    {
        if (contentType != TextContentTypes.MD)
            return ChunkPlainText(markdown);

        return ChunkMarkdown(markdown);
    }

    private static List<string> ChunkMarkdown(string markdown)
    {
        var (metadata, body) = FrontmatterParser.Extract(markdown);
        var metadataPrefix = BuildMetadataPrefix(metadata);

        var sections = SplitIntoSections(body);
        sections = MergeTrivialSections(sections);

        var chunks = new List<string>();

        foreach (var section in sections)
        {
            var breadcrumbLine = section.Breadcrumb.Length > 0
                ? $"[Section: {section.Breadcrumb}]\n\n"
                : string.Empty;

            var prefix = metadataPrefix + breadcrumbLine;
            var withPrefix = prefix + section.Content;

            if (EstimateTokens(withPrefix) <= MaxTokens)
            {
                chunks.Add(withPrefix);
                continue;
            }

            var lines = TextChunker.SplitMarkDownLines(section.Content, MaxTokens);
            var paragraphs = TextChunker.SplitMarkdownParagraphs(lines, MaxTokens, Overlap);
            chunks.AddRange(paragraphs.Select(p => prefix + p));
        }

        return chunks;
    }

    private static string BuildMetadataPrefix(Dictionary<string, string> metadata)
    {
        if (metadata.Count == 0) return string.Empty;

        var service = metadata.GetValueOrDefault("service");
        var category = metadata.GetValueOrDefault("category");

        if (service is null && category is null) return string.Empty;

        return $"[Document: {service} | Category: {category}]\n";
    }

    private static int EstimateTokens(string text) => text.Length / 4; // rough heuristic

    private static List<string> ChunkPlainText(string text)
    {
        var lines = TextChunker.SplitPlainTextLines(text, MaxTokens);
        return TextChunker.SplitPlainTextParagraphs(lines, MaxTokens, Overlap);
    }

    private sealed record MarkdownSection(string Breadcrumb, string Content);

    private static List<MarkdownSection> SplitIntoSections(string markdown)
    {
        var sections = new List<MarkdownSection>();
        var builder = new StringBuilder();

        string? currentH1 = null;
        string? currentH2 = null;
        string? pendingBreadcrumb = null;

        using var reader = new StringReader(markdown);
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            var h1Match = Regex.Match(line, @"^#\s+(.*)");
            var h2Match = Regex.Match(line, @"^##\s+(.*)");

            if (h1Match.Success || h2Match.Success)
            {
                if (builder.Length > 0)
                {
                    sections.Add(new MarkdownSection(pendingBreadcrumb ??
                        string.Empty, builder.ToString().Trim()));
                    builder.Clear();
                }

                if (h1Match.Success)
                {
                    currentH1 = h1Match.Groups[1].Value.Trim();
                    currentH2 = null;
                }
                else
                {
                    currentH2 = h2Match.Groups[1].Value.Trim();
                }

                pendingBreadcrumb = BuildBreadcrumb(currentH1, currentH2);
            }

            builder.AppendLine(line);
        }

        if (builder.Length > 0)
            sections.Add(new MarkdownSection(pendingBreadcrumb ??
                string.Empty, builder.ToString().Trim()));

        return sections;
    }

    private static string BuildBreadcrumb(string? h1, string? h2)
    {
        if (h1 is null) return string.Empty;
        return h2 is null ? h1 : $"{h1} > {h2}";
    }

    private static List<MarkdownSection> MergeTrivialSections(List<MarkdownSection> sections)
    {
        var merged = new List<MarkdownSection>();

        foreach (var section in sections)
        {
            if (merged.Count > 0 && IsTrivial(merged[^1].Content))
            {
                var previous = merged[^1];
                merged[^1] = previous with
                {
                    Content = previous.Content.TrimEnd() + "\n\n" + section.Content,
                    // keep the deeper/more specific breadcrumb if the merged-in section has one
                    Breadcrumb = string.IsNullOrEmpty(section.Breadcrumb) ? previous.Breadcrumb : section.Breadcrumb
                };
            }
            else
            {
                merged.Add(section);
            }
        }

        if (merged.Count > 1 && IsTrivial(merged[^1].Content))
        {
            var last = merged[^1];
            merged.RemoveAt(merged.Count - 1);
            var previous = merged[^1];
            merged[^1] = previous with { Content = previous.Content.TrimEnd() + "\n\n" + last.Content };
        }

        return merged;
    }

    private static bool IsTrivial(string section)
    {
        var wordCount = section
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !HeaderRegex().IsMatch(l) && l.Trim() != "---")
            .SelectMany(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Count();

        return wordCount < MinSectionWords;
    }

    [GeneratedRegex(@"^#{1,6}\s")]
    private static partial Regex HeaderRegex();
}
