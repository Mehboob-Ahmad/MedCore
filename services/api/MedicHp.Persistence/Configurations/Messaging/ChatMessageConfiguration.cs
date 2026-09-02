using MedicHp.Domain.Entities.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Messaging;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages", "messaging");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.Property(x => x.MessageType).HasDefaultValue("TEXT").HasMaxLength(20);
        builder.Property(x => x.Content).IsRequired(false);
        
        builder.HasOne(x => x.Attachment)
               .WithMany()
               .HasForeignKey(x => x.AttachmentId)
               .OnDelete(DeleteBehavior.SetNull);
               
        builder.HasIndex(x => new { x.ConversationId, x.SentAt });
        builder.HasIndex(x => new { x.ConversationId, x.IsRead });
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
