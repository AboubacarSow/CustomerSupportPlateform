using CustomerSupportPlateform.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.EventHandlers;

public class KnowledgeDocumentFailedIndexingEventHandler(IApplicationDbContext dbContext) : INotificationHandler<KnowledgeDocumentFailedIndexingEvent>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    public async Task Handle(KnowledgeDocumentFailedIndexingEvent notification, CancellationToken cancellationToken)
    {
        var document = await _dbContext.KnowledgeDocuments.FirstOrDefaultAsync(x=>x.Id == notification.DocumentId,cancellationToken)
                        ?? throw new KnowledgeDocumentNotFoundException($"KnowledgeDocument with Id: {notification.DocumentId} not found");

        document.MarkAsFailed();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

