using MedicHp.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Clinical;

public class ConsultationTemplateConfiguration : IEntityTypeConfiguration<ConsultationTemplate>
{
    public void Configure(EntityTypeBuilder<ConsultationTemplate> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.TemplateName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Diagnosis)
            .HasMaxLength(500);

        builder.Property(t => t.ClinicalNotes)
            .HasMaxLength(2000);

        builder.Property(t => t.TreatmentPlan)
            .HasMaxLength(2000);

        builder.Property(t => t.FollowUpInstructions)
            .HasMaxLength(1000);

        builder.HasOne(t => t.Doctor)
            .WithMany()
            .HasForeignKey(t => t.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(t => !t.IsDeleted);
        
        builder.HasIndex(t => new { t.DoctorId, t.TemplateName }).IsUnique();
    }
}
