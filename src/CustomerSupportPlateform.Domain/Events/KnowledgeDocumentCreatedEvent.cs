
namespace CustomerSupportPlateform.Domain.Events;

public record KnowledgeDocumentCreatedEvent(Guid DocumentId,string TmpfilePath) : IDomainEvent;
