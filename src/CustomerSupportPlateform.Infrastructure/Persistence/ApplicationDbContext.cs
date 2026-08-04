


using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CustomerSupportPlateform.Infrastructure.Persistence;


internal class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
        modelBuilder.HasPostgresExtension("vector");
        base.OnModelCreating(modelBuilder);
    }

    async Task IApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken)
    {
         await base.SaveChangesAsync(cancellationToken);

    }

    void IApplicationDbContext.Add<TEntity>(TEntity entity)
    {
        base.Add<TEntity>(entity);
    }

 
    void IApplicationDbContext.Remove<TEntity>(TEntity entity)
    {
        base.Remove<TEntity>(entity);
    }

    internal DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();
    internal DbSet<Session> Sessions => Set<Session>();
    internal DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();
    internal DbSet<DocumentChunk> Chunks => Set<DocumentChunk>();

    internal DbSet<OutBoxMessage> OutBoxMessages => Set<OutBoxMessage>();

    IQueryable<ConversationMessage> IApplicationDbContext.ConversationMessages => ConversationMessages;
    IQueryable<Session> IApplicationDbContext.Sessions => Sessions;
    IQueryable<KnowledgeDocument> IApplicationDbContext.KnowledgeDocuments => KnowledgeDocuments;
    IQueryable<DocumentChunk> IApplicationDbContext.Chunks => Chunks;
}
