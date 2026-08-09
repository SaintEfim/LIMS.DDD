using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

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

        builder.OwnsOne(x => x.ParameterSnapshot, snap =>
        {
            snap.Property(x => x.InputParameterId)
                .HasConversion(id => id.Value, value => new ParameterTemplateId(value));

            snap.Property(p => p.InputParameterId)
                .HasColumnName("ParameterId");

            snap.Property(p => p.Name)
                .HasConversion(n => n.Value, n => Name.Create(n)
                    .GetValue())
                .HasColumnName("ParamName")
                .HasMaxLength(100)
                .IsRequired();

            snap.Property(p => p.AliasName)
                .HasConversion(a => a.Value, a => AliasName.Create(a)
                    .GetValue())
                .HasColumnName("ParamAliasName")
                .HasMaxLength(100)
                .IsRequired();

            snap.OwnsOne(x => x.Specification, spec =>
            {
                spec.Property(p => p.MinValue)
                    .HasColumnName("SpecMinValue");
                spec.Property(p => p.MaxValue)
                    .HasColumnName("SpecMaxValue");
            });
        });
    }
}
