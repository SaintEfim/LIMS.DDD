using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.LaboratoryOperations.Persistence.Configurations;

public class MeasuredValueConfiguration : IEntityTypeConfiguration<MeasuredValue>
{
    public void Configure(
        EntityTypeBuilder<MeasuredValue> builder)
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

        builder.Property(x => x.Value)
            .HasColumnType("decimal(18,6)");

        builder.Property(x => x.InputParameterId)
            .HasConversion(id => id.Value, v => new InputParameterId(v))
            .IsRequired();

        builder.HasOne<InputParameterSnapshot>()
            .WithMany()
            .HasForeignKey(x => x.InputParameterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
