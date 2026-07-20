namespace CustomerSupportPlateform.Domain.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
