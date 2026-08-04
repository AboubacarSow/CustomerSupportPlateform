using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportPlateform.Infrastructure.Persistence.Configurations;


public class KnowledgeDocumentConfiguration : IEntityTypeConfiguration<KnowledgeDocument>
{
    public void Configure(EntityTypeBuilder<KnowledgeDocument> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(k => k.Language)
            .HasConversion<string>();

        builder.Property(k => k.Status)
            .HasConversion<string>();
    }
}

