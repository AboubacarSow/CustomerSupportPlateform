
using CustomerSupportPlateform.Domain.Constants;

namespace CustomerSupportPlateform.Domain.Events;

public record KnowledgeDocumentCreatedEvent(Guid DocumentId,
                    string DocumentFormat,
                    string TmpfilePath) : IDomainEvent;
