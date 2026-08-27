using MedicHp.Domain.Entities.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Messaging;

public class WhatsAppMessageConfiguration : IEntityTypeConfiguration<WhatsAppMessage>
{
    public void Configure(EntityTypeBuilder<WhatsAppMessage> builder)
    {
        builder.ToTable("WhatsAppMessages", "messaging");

        builder.HasKey(e => e.Id);

        // Meta wamid must be unique for idempotency
        builder.HasIndex(e => e.WhatsAppMessageId).IsUnique();
        
        builder.HasIndex(e => e.PhoneNumber);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Direction);

        builder.Property(e => e.WhatsAppMessageId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.MessageType)
            .IsRequired();

        builder.Property(e => e.Direction)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(1000);

        // Allow nulls for Metadata, but limit length if provided
        builder.Property(e => e.Metadata)
            .HasMaxLength(4000); // Only for small contextual JSON

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull); // Keep message even if user is deleted

        builder.HasOne(x => x.DoctorProfile)
            .WithMany()
            .HasForeignKey(x => x.DoctorProfileId)
            .OnDelete(DeleteBehavior.SetNull); // Don't allow deleting a connection that has messages
    }
}
