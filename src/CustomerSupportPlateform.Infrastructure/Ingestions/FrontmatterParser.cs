using System.Text.RegularExpressions;

namespace CustomerSupportPlateform.Infrastructure.Ingestions;

internal static partial class FrontmatterParser
{
    private static readonly Regex FrontmatterPattern =
        SingleLineRegex();

    public static (Dictionary<string, string> Metadata, string Body) Extract(string markdown)
    {
        var match = FrontmatterPattern.Match(markdown);
        if (!match.Success)
            return (new Dictionary<string, string>(), markdown);

        var metadata = new Dictionary<string, string>();
        var yaml = match.Groups[1].Value;

        foreach (var line in yaml.Split('\n'))
        {
            var kv = line.Split(':', 2);
            if (kv.Length != 2) continue;
            var key = kv[0].Trim();
            var value = kv[1].Trim();
            if (!string.IsNullOrEmpty(value))
                metadata[key] = value;
        }

        var body = markdown[match.Length..];
        return (metadata, body);
    }

    [GeneratedRegex(@"\A---\s*\n(.*?)\n---\s*\n?", RegexOptions.Singleline)]
    private static partial Regex SingleLineRegex();
}