namespace CustomerSupportPlateform.Domain.Exceptions;

public class KnowledgeDocumentNotFoundException(string message) : NotFoundException(message)
{
}