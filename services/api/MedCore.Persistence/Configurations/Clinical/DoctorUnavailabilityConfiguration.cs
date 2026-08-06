using MedCore.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedCore.Persistence.Configurations.Clinical;

public class DoctorUnavailabilityConfiguration : IEntityTypeConfiguration<DoctorUnavailability>
{
    public void Configure(EntityTypeBuilder<DoctorUnavailability> builder)
    {
        builder.ToTable("DoctorUnavailabilitys", "Clinical".ToLower());
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
