namespace CustomerSupportPlateform.Domain.Events;

public record KnowledgeDocumentIndexingEvent(Guid DocumentId):IDomainEvent;
