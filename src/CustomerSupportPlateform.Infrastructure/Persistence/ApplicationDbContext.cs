


namespace CustomerSupportPlateform.Infrastructure.Persistence;


public class ApplicationDbContext : DbContext, IApplicationDbContext
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

    public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();
    public DbSet<DocumentChunk> Chunks => Set<DocumentChunk>();

    IQueryable<ConversationMessage> IApplicationDbContext.ConversationMessages => ConversationMessages;
    IQueryable<Session> IApplicationDbContext.Sessions => Sessions;
    IQueryable<KnowledgeDocument> IApplicationDbContext.KnowledgeDocuments => KnowledgeDocuments;
    IQueryable<DocumentChunk> IApplicationDbContext.Chunks => Chunks;
}
