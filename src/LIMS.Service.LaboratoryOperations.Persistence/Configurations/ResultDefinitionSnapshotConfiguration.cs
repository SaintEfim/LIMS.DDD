using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.LaboratoryOperations.Persistence.Configurations;

public class ResultDefinitionSnapshotConfiguration : IEntityTypeConfiguration<ResultDefinitionSnapshot>
{
    public void Configure(
        EntityTypeBuilder<ResultDefinitionSnapshot> builder)
    {
        builder.ToTable("ResultDefinitionSnapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new ResultDefinitionId(value));

        builder.Property(x => x.StudyTemplateId)
            .HasConversion(id => id.Value, value => new StudyTemplateId(value));

        builder.Property(x => x.ResultInstance)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.UnitId)
            .HasConversion(id => id.Value, value => new UnitId(value));

        builder.HasOne<UnitSnapshot>()
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

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
