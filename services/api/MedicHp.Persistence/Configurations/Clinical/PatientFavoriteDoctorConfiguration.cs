using MedicHp.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Clinical;

public class PatientFavoriteDoctorConfiguration : IEntityTypeConfiguration<PatientFavoriteDoctor>
{
    public void Configure(EntityTypeBuilder<PatientFavoriteDoctor> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(x => x.Doctor)
            .WithMany()
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(x => new { x.PatientId, x.DoctorId }).IsUnique().HasFilter("[IsDeleted] = 0");
            
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
