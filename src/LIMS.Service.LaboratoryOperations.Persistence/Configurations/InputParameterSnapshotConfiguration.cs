using LIMS.Service.LaboratoryOperations.Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.LaboratoryOperations.Persistence.Configurations;

public class InputParameterSnapshotConfiguration : IEntityTypeConfiguration<InputParameterSnapshot>
{
    public void Configure(EntityTypeBuilder<InputParameterSnapshot> builder)
    {
        builder.ToTable("InputParameterSnapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new InputParameterId(value));

        builder.Property(x => x.StudyTemplateId)
            .HasConversion(id => id.Value, value => new StudyTemplateId(value));

        builder.Property(x => x.Name)
            .HasConversion(n => n.Value, n => Name.Create(n).GetValue())
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(d => d.Value, d => Description.Create(d).GetValue())
            .HasMaxLength(2000);

        builder.Property(x => x.AliasName)
            .HasConversion(a => a.Value, a => AliasName.Create(a).GetValue())
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(x => x.Specification, spec =>
        {
            spec.Property(s => s.MinValue)
                .HasColumnName("MinValue")
                .HasColumnType("decimal(18,6)");

            spec.Property(s => s.MaxValue)
                .HasColumnName("MaxValue")
                .HasColumnType("decimal(18,6)");
        });
    }
}
