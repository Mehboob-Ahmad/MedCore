using MedCore.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedCore.Persistence.Configurations.Clinical;

public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.ToTable("Consultations", "clinical");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.HasIndex(x => x.AppointmentId).IsUnique().HasFilter("\"IsDeleted\" = false");
        
        builder.Property(x => x.VisitType).HasMaxLength(50);
        builder.Property(x => x.FollowUpUrgency).HasMaxLength(50);
        builder.Property(x => x.PrivateNotes).HasMaxLength(2000);
        builder.Property(x => x.PatientNotes).HasMaxLength(2000);
        builder.Property(x => x.FollowUpInstructions).HasMaxLength(1000);
        
        builder.HasOne(x => x.Vitals)
            .WithOne(v => v.Consultation)
            .HasForeignKey<ConsultationVital>(v => v.ConsultationId);
            
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
