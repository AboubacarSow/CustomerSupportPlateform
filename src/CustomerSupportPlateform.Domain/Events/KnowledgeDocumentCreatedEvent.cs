
using CustomerSupportPlateform.Domain.Constants;

namespace CustomerSupportPlateform.Domain.Events;

public record KnowledgeDocumentCreatedEvent(Guid DocumentId,
                    IngestionDocumentFormat DocumentFormat,
                    string TmpfilePath) : IDomainEvent;
