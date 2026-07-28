
using CustomerSupportPlateform.Domain.Constants;

namespace CustomerSupportPlateform.Domain.Events;

public record KnowledgeDocumentCreatedEvent(Guid DocumentId,
                    string ContentType,
                    string LocalPath) : IDomainEvent;
