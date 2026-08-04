using System.Net.Mime;

namespace CustomerSupportPlateform.Domain.Constants;

public static class TextContentTypes 
{
    public const string PDF = MediaTypeNames.Application.Pdf;
    public const string MD = MediaTypeNames.Text.Markdown;
    public const string DOCX = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    public const string TEXT = MediaTypeNames.Text.Plain;
}


public enum Language
{
    English = 1,
    Turkish = 2
}
