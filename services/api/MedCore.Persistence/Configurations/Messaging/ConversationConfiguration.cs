using MedCore.Domain.Entities.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedCore.Persistence.Configurations.Messaging;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations", "messaging");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.HasIndex(x => new { x.PatientId, x.DoctorId }).IsUnique().HasFilter("\"IsDeleted\" = false");
        builder.HasIndex(x => new { x.PatientId, x.LastMessageAt });
        builder.HasIndex(x => new { x.DoctorId, x.LastMessageAt });
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
