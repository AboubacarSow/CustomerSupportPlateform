namespace CustomerSupportPlateform.Application.KnowledgeDocuments.Features.Dtos;

public record DocumentDto(Guid Id,
                        string Title,
                        string? Description,
                        string ContentType,
                        string OriginalFileName,
                        long FileSize,
                        string Status,
                        DateTimeOffset?  IndexedAt
                        );
