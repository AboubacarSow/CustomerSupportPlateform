using Microsoft.SemanticKernel.Text;

namespace CustomerSupportPlateform.Infrastructure.Ingestions;

internal class ContentChunker : IContentChunker
{
    private readonly int _maxTokensPerChunk = 500;
    private readonly int _overlap = 50;
    public List<string> Chunk(string content)
    {
        var lines = TextChunker.SplitPlainTextLines(content, _maxTokensPerChunk);
        var chunks = TextChunker.SplitPlainTextParagraphs(lines, _overlap);

        return chunks;
    }
}
