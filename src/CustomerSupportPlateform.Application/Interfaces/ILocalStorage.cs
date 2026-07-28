namespace CustomerSupportPlateform.Application.Interfaces;


public interface ILocalStorage
{
    void ClearDocumentFromTemp(string path);
    Task<string> UploadFileToTempAsync(IFormFile file);


}
