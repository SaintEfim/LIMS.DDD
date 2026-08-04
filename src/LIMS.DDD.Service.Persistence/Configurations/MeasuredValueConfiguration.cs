using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class MeasuredValueConfiguration : IEntityTypeConfiguration<MeasuredValue>
{
    public void Configure(EntityTypeBuilder<MeasuredValue> builder)
    {
        builder.ToTable("MeasuredValues");

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new MeasuredValueId(value));

        builder.Property(x => x.StudyId)
            .HasConversion(id => id.Value, value => new StudyId(value));

        builder.Property(x => x.ParameterId)
            .HasConversion(id => id.Value, value => new InputParameterId(value));

        builder.Property(x => x.Value)
            .HasColumnType("decimal(18,6)");
    }
}
