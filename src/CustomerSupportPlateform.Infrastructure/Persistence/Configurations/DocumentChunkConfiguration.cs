using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportPlateform.Infrastructure.Persistence.Configurations;

public class DocumentChunkConfiguration : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.HasKey(c => c.Id);


        builder.Property(c=>c.Language)
            .HasConversion<int>();

        builder.HasIndex(c=>c.Language);


        builder.HasOne(c => c.Document)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Embedding)
            .HasColumnType("vector(768)");
    }
}

