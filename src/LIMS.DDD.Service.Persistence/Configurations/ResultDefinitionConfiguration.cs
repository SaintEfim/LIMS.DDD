using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ResultDefinitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class ResultDefinitionConfiguration : IEntityTypeConfiguration<ResultDefinition>
{
    public void Configure(
        EntityTypeBuilder<ResultDefinition> builder)
    {
        builder.ToTable("ResultDefinitions");

        builder.HasKey(x => x.Id);

         builder.Property(x => x.Id)
            .HasConversion(id => id.Value, id => new ResultDefinitionId(id));

        builder.Property(x => x.StudyTemplateId)
            .HasConversion(id => id.Value, id => new StudyTemplateId(id));

        builder.Property(x => x.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(x => x.Specification, vr =>
        {
            vr.Property(p => p.MaxValue);
            vr.Property(p => p.MinValue);
        });

        builder.HasIndex(x => new
            {
                x.StudyTemplateId,
                x.Unit,
                x.ResultInstance
            })
            .IsUnique();
    }
}
