namespace CustomerSupportPlateform.Application.Interfaces;


public interface ITempStorageService
{
    void ClearDocumentFromTemp(string path);
    Task<string> UploadFileToTempAsync(IFormFile file);
}
