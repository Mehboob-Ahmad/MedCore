using MedCore.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedCore.Persistence.Configurations.Core;

public class FileConfiguration : IEntityTypeConfiguration<MedCore.Domain.Entities.Core.File>
{
    public void Configure(EntityTypeBuilder<MedCore.Domain.Entities.Core.File> builder)
    {
        builder.ToTable("Files", "core");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.HasOne(x => x.UploadedByUser)
            .WithMany()
            .HasForeignKey(x => x.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
