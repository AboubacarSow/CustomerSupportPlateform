using Microsoft.AspNetCore.Http;

namespace CustomerSupportPlateform.Infrastructure.Storage;


internal class LocalStorage : ILocalStorage
{
    private readonly string localstrogefolder = "data/localblob";

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

    public async Task<string> UploadFileToTempAsync(IFormFile file)
    {
        var folder = Path.Combine(Directory.GetCurrentDirectory(), localstrogefolder);
        var path = Path.Combine(folder, $"{file.FileName}");
        using var stream = new FileStream(path,FileMode.Create);

        await file.CopyToAsync(stream);

        return path;
    }
}