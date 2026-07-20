using CustomerSupportPlateform.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportPlateform.Application.KnowledgeDocuments.EventHandlers;

public class KnowledgeDocumentIndexingEventHandler(IApplicationDbContext dbContext) : INotificationHandler<KnowledgeDocumentIndexingEvent>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    public async Task Handle(KnowledgeDocumentIndexingEvent notification, CancellationToken cancellationToken)
    {
        var document = await _dbContext.KnowledgeDocuments.FirstOrDefaultAsync(d=>d.Id == notification.DocumentId, cancellationToken)
                    ?? throw new KnowledgeDocumentNotFoundException($"KnowledgeDocument with Id: {notification.DocumentId} not found");

        document.MarkAsIndexed();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

