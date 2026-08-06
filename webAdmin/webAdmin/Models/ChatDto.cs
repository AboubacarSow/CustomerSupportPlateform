namespace webAdmin.Models;

public record ChatRequestDto(Guid SessionId, string Question);
public record ChatResponseDto(string Message);

public record DocumentDto(Guid Id,
                        string Title,
                        string? Description,
                        string ContentType,
                        string OriginalFileName,
                        long FileSize,
                        string Status,
                        DateTimeOffset?  IndexedAt
                        );
public record KnowledgeDocumentItem(Guid Id,
                        string Title,
                        string? Description,
                        string ContentType,
                        string OriginalFileName,
                        string StoragePath,
                        long FileSize,
                        string Status,
                        DateTimeOffset?  IndexedAt,
                        string Language
                        );


public record LoginDto(string Email, string Password);
public record RegisterDto(string FirstName, string LastName, string Email, string Password);


public record CurrentUserDto(Guid Id, string Email, IEnumerable<string> Roles);

public enum Language
{
    English = 1,
    Turkish =2
}




