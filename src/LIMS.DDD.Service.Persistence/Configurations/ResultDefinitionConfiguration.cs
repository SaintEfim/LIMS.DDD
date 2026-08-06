using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;
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
            .HasConversion(id => id.Value, id => new StudyTemplateId(id))
            .IsRequired();

        builder.Property(x => x.ResultInstance)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(x => x.Specification, spec =>
        {
            spec.Property(p => p.MinValue)
                .HasColumnName("SpecMinValue");
            spec.Property(p => p.MaxValue)
                .HasColumnName("SpecMaxValue");
        });

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => new
            {
                x.StudyTemplateId,
                x.ResultInstance,
                x.Unit
            })
            .IsUnique().HasFilter("\"IsDeleted\" = false");
    }
}
