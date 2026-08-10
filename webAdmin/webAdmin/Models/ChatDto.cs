using System.ComponentModel.DataAnnotations;

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
                        DateTimeOffset? IndexedAt
                        );
public record KnowledgeDocumentItem(Guid Id,
                        string Title,
                        string? Description,
                        string ContentType,
                        string OriginalFileName,
                        string StoragePath,
                        long FileSize,
                        string Status,
                        DateTimeOffset? IndexedAt,
                        string Language
                        );


public record LoginDto(string Email, string Password)
{

}
public record RegisterDto
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public RegisterDto(string firstName,string lastName,string email, string password,string confirmPassword)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
    }

    public RegisterDto() { 
    }
}


public record CurrentUserDto(Guid Id, string Email, IEnumerable<string> Roles);

public enum Language
{
    English = 1,
    Turkish = 2
}




