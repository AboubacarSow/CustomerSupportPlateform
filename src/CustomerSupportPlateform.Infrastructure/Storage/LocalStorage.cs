using Microsoft.AspNetCore.Http;

namespace CustomerSupportPlateform.Infrastructure.Storage;


internal class LocalStorage : ILocalStorage
{
    private readonly string localstrogefolder = "data/localblob";
    private readonly string englishFolder = "en";
    private readonly string turkishFolder = "tr";

    public LocalStorage()
    {
        var folder = Path.Combine(Directory.GetCurrentDirectory(), localstrogefolder);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }
    public void ClearDocumentFromTemp(string path)
    {
        File.Delete(path);
    }

    public async Task<string> UploadFileToTempAsync(IFormFile file, Language language)
    {
        
        var folder = Path.Combine(Directory.GetCurrentDirectory(), 
            localstrogefolder,
            language == Language.Turkish ? turkishFolder : englishFolder);
        var path = Path.Combine(folder, $"{file.FileName}");
        using var stream = new FileStream(path,FileMode.Create);

        await file.CopyToAsync(stream);

        return path;
    }
}