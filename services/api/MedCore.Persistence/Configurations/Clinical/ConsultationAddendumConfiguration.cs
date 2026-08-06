using MedCore.Domain.Entities.Clinical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedCore.Persistence.Configurations.Clinical;

public class ConsultationAddendumConfiguration : IEntityTypeConfiguration<ConsultationAddendum>
{
    public void Configure(EntityTypeBuilder<ConsultationAddendum> builder)
    {
        builder.ToTable("ConsultationAddendums", "Clinical".ToLower());
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
