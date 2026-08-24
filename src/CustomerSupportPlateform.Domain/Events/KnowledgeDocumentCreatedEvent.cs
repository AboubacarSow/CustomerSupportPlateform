namespace CustomerSupportPlateform.Domain.Events;

public record KnowledgeDocumentCreatedEvent(Guid DocumentId,
                    string ContentType,
                    string LocalPath) : IDomainEvent;

public record KnowledgeDocumentContentUpgradedEvent(Guid DocumentId,
                    string ContentType,
                    string LocalPath) : IDomainEvent;


public record KnowledgeDocumentFailedIndexingEvent(Guid DocumentId): IDomainEvent;
