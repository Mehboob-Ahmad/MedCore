using MedicHp.Domain.Entities.Lookup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicHp.Persistence.Configurations.Lookup;

public class DiseaseSpecializationConfiguration : IEntityTypeConfiguration<DiseaseSpecialization>
{
    public void Configure(EntityTypeBuilder<DiseaseSpecialization> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Disease)
            .WithMany(x => x.DiseaseSpecializations)
            .HasForeignKey(x => x.DiseaseId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(x => x.Specialization)
            .WithMany(x => x.DiseaseSpecializations)
            .HasForeignKey(x => x.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
