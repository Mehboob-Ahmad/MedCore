using MedicHp.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Clinical;

public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
{
    public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
    {
        builder.ToTable("PrescriptionItems", "Clinical".ToLower());
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.Property(x => x.Strength).HasMaxLength(100);
        builder.Property(x => x.Route).HasMaxLength(100);
        builder.Property(x => x.Timing).HasMaxLength(100);
        builder.Property(x => x.Quantity).HasMaxLength(100);
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
