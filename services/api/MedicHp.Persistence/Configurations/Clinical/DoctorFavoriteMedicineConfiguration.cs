using MedicHp.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Clinical;

public class DoctorFavoriteMedicineConfiguration : IEntityTypeConfiguration<DoctorFavoriteMedicine>
{
    public void Configure(EntityTypeBuilder<DoctorFavoriteMedicine> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.MedicationName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(f => f.Doctor)
            .WithMany()
            .HasForeignKey(f => f.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => new { f.DoctorId, f.MedicationName }).IsUnique();
    }
}
