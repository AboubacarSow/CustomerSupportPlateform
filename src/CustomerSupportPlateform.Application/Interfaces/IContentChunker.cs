namespace CustomerSupportPlateform.Application.Interfaces;

public interface IContentChunker
{
    List<string> Chunk(string content);
}
