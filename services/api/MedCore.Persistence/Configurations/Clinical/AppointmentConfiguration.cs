using MedCore.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedCore.Persistence.Configurations.Clinical;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments", "clinical");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.BookingNote).HasMaxLength(2000);
        builder.Property(x => x.CancellationReason).HasMaxLength(1000);
        builder.Property(x => x.DoctorNotes).HasMaxLength(4000);
        
        builder.HasIndex(x => new { x.DoctorId, x.ScheduledAt });
        builder.HasIndex(x => new { x.PatientId, x.Status });
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.ExpiresAt, x.Status });
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
