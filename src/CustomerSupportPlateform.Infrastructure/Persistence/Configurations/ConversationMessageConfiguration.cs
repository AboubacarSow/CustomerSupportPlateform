using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportPlateform.Infrastructure.Persistence.Configurations;

public class ConversationMessageConfiguration : IEntityTypeConfiguration<ConversationMessage>
{
    public void Configure(EntityTypeBuilder<ConversationMessage> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.Session)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}

