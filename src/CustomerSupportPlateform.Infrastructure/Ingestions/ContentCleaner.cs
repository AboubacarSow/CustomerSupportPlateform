using System.Text.RegularExpressions;

namespace CustomerSupportPlateform.Infrastructure.Ingestions;

internal partial class ContentCleaner : IContentCleaner
{
    
    [GeneratedRegex(@"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", RegexOptions.Compiled)]
    private static partial Regex ControlChars();

    [GeneratedRegex(@"[•·†‡°™®©℠€£¥¢ƒ¤§¶‖†‡‣⁎↓↑→←⇒⇐⇑⇓◄►★☆✓✗✘✔◇◆▪▫►◄║═╔╗╚╝╠╣╦╩╬├┤┬┴┼─│┌┐└┘├┤┬┴┼]", RegexOptions.Compiled)]
    private static partial Regex SpecialSymbols();

    [GeneratedRegex(@"https?://\S+|www\.\S+", RegexOptions.Compiled)]
    private static partial Regex Urls();

    [GeneratedRegex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", RegexOptions.Compiled)]
    private static partial Regex Emails();

    [GeneratedRegex(@"[!?;:]{2,}", RegexOptions.Compiled)]
    private static partial Regex RepeatedPunctuation();

    [GeneratedRegex(@"[^a-zA-Z0-9\s\.\,\!\?\:\-\(\)\'\""]", RegexOptions.Compiled)]
    private static partial Regex NonStandardChars();

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex MultipleSpaces();

    public string Clean(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;

        content = ControlChars().Replace(content, " ");
        content = SpecialSymbols().Replace(content, " ");
        content = Urls().Replace(content, " ");
        content = Emails().Replace(content, " ");

        content = RepeatedPunctuation().Replace(content, ".");

        content = NonStandardChars().Replace(content, " ");

        content = MultipleSpaces().Replace(content, " ").Trim();

        return content;
    }
   
}



