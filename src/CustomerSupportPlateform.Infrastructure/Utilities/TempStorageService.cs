using Microsoft.AspNetCore.Http;

namespace CustomerSupportPlateform.Infrastructure.Utilities;


internal class TempStorageService : ITempStorageService
{
    private readonly string folder = "data/tmp";

    public TempStorageService()
    {
        if (!File.Exists(folder))
        {
            File.Create(folder);
        }
    }
    public void ClearDocumentFromTemp(string path)
    {
        File.Delete(path);
    }

    public async Task<string> UploadFileToTempAsync(IFormFile file)
    {

        var path = Path.Combine(folder, $"{file.FileName}_{Guid.NewGuid()}");
        using var stream = new FileStream(path,FileMode.Create,FileAccess.Write);

        await file.CopyToAsync(stream);

        return path;
    }
}