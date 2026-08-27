using MedicHp.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Clinical;

public class PrescriptionTemplateItemConfiguration : IEntityTypeConfiguration<PrescriptionTemplateItem>
{
    public void Configure(EntityTypeBuilder<PrescriptionTemplateItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.MedicationName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Strength)
            .HasMaxLength(100);

        builder.Property(i => i.Dosage)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Frequency)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Duration)
            .HasMaxLength(100);

        builder.Property(i => i.Route)
            .HasMaxLength(100);

        builder.Property(i => i.Timing)
            .HasMaxLength(100);

        builder.Property(i => i.Quantity)
            .HasMaxLength(50);

        builder.Property(i => i.Instructions)
            .HasMaxLength(500);
    }
}
