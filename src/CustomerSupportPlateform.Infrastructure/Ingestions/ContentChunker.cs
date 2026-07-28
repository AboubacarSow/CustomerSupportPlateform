using Microsoft.SemanticKernel.Text;

namespace CustomerSupportPlateform.Infrastructure.Ingestions;

internal class ContentChunker : IContentChunker
{
    private readonly int _maxTokensPerChunk = 500;
    private readonly int _overlap = 50;
    public List<string> Chunk(string content, string contentType)
    {
        List<string>? lines = contentType switch
        {
            TextContentTypes.MD => TextChunker.SplitMarkDownLines(content, _maxTokensPerChunk),
            _                   => TextChunker.SplitPlainTextLines(content, _maxTokensPerChunk),

        };

        var chunks = contentType switch
        {
            TextContentTypes.MD => TextChunker.SplitMarkdownParagraphs(lines, maxTokensPerParagraph: _maxTokensPerChunk,
                                                                        overlapTokens: _overlap),
            _                   => TextChunker.SplitPlainTextParagraphs(lines,
                                                                        maxTokensPerParagraph:_maxTokensPerChunk,
                                                                        overlapTokens:_overlap),   
        };
       

        return chunks;
    }
}
