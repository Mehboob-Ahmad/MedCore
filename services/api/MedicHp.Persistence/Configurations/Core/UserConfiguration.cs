using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Core;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "core");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.HasIndex(x => x.NormalizedEmail).IsUnique().HasFilter("\"IsDeleted\" = false");
        builder.HasIndex(x => x.PhoneNumber).IsUnique().HasFilter("\"IsDeleted\" = false");
        builder.HasIndex(x => x.IsActive);
        
        builder.HasOne(x => x.PatientProfile)
            .WithOne(p => p.User)
            .HasForeignKey<PatientProfile>(p => p.UserId);

        builder.HasOne(x => x.DoctorProfile)
            .WithOne(d => d.User)
            .HasForeignKey<DoctorProfile>(d => d.UserId);
            
        builder.HasOne(x => x.ProfilePhotoFile)
            .WithMany()
            .HasForeignKey(x => x.ProfilePhotoFileId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
