using MedicHp.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Clinical;

public class DoctorPaymentMethodConfiguration : IEntityTypeConfiguration<DoctorPaymentMethod>
{
    public void Configure(EntityTypeBuilder<DoctorPaymentMethod> builder)
    {
        builder.ToTable("DoctorPaymentMethods", "clinical");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

        builder.Property(x => x.PaymentMethodType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.PaymentProvider).HasMaxLength(200);
        builder.Property(x => x.AccountTitle).HasMaxLength(200);
        builder.Property(x => x.AccountNumber).HasMaxLength(100);
        builder.Property(x => x.IBAN).HasMaxLength(50);

        builder.HasIndex(x => new { x.DoctorProfileId, x.IsActive });

        builder.HasOne(x => x.DoctorProfile)
            .WithMany(x => x.PaymentMethods)
            .HasForeignKey(x => x.DoctorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
