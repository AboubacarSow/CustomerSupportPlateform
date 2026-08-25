namespace CustomerSupportPlateform.Domain.Exceptions;

public class KnowledgeDocumentNotFoundException(string message) : NotFoundException(message)
{
}

public class FileUploadedExtensionException(string extensions):
    Exception($"Uploaded File extension must be in :{extensions}")
{

}