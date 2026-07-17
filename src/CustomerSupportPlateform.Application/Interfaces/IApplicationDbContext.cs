using CustomerSupportPlateform.Domain.Entities;

namespace CustomerSupportPlateform.Application.Interfaces;

public interface IApplicationDbContext
{
     IQueryable<ConversationMessage> ConversationMessages {  get; }
     IQueryable<Session> Sessions {  get; }
     IQueryable<KnowledgeDocument> KnowledgeDocuments { get; }
     IQueryable<DocumentChunk> Chunks { get; }

     Task SaveChangesAsync(CancellationToken cancellationToken= default);

    void Add<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;
}
