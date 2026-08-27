using MedicHp.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Clinical;

public class PrescriptionTemplateConfiguration : IEntityTypeConfiguration<PrescriptionTemplate>
{
    public void Configure(EntityTypeBuilder<PrescriptionTemplate> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.TemplateName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Notes)
            .HasMaxLength(1000);

        builder.HasOne(t => t.Doctor)
            .WithMany()
            .HasForeignKey(t => t.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Items)
            .WithOne(i => i.PrescriptionTemplate)
            .HasForeignKey(i => i.PrescriptionTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(t => !t.IsDeleted);
        
        builder.HasIndex(t => new { t.DoctorId, t.TemplateName }).IsUnique();
    }
}
