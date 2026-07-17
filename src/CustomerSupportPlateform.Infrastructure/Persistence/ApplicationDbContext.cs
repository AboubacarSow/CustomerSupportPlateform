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

    public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();
    public DbSet<Session> Sessions  => Set<Session>();
    public DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();
    public DbSet<DocumentChunk> Chunks => Set<DocumentChunk>();
}