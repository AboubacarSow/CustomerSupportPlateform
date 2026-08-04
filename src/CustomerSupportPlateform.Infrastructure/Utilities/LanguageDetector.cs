namespace CustomerSupportPlateform.Infrastructure.PromptPreparation;


public static class LanguageDetector
{
    private static readonly char[] TurkishSpecificChars = { 'ç', 'ğ', 'ı', 'ö', 'ş', 'ü', 'Ç', 'Ğ', 'İ', 'Ö', 'Ş', 'Ü' };

    public static Language Detect(string text)
    {
        return text.Any(c => TurkishSpecificChars.Contains(c)) ? Language.Turkish : Language.English;
    }
}