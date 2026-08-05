using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
{
    public void Configure(
        EntityTypeBuilder<TestResult> builder)
    {
        builder.ToTable("TestResults");

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
        builder.Property(x => x.DeletedAt)
            .IsRequired(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new TestResultId(value));

        builder.Property(x => x.StudyId)
            .HasConversion(id => id.Value, value => new StudyId(value));

        builder.Property(x => x.Value)
            .HasColumnType("decimal(18,6)");

        builder.Property(x => x.IsOutOfSpec)
            .IsRequired();

        builder.OwnsOne(x => x.ResultSnapshot, snap =>
        {
            snap.Property(p => p.ResultDefinitionId)
                .HasColumnName("ResultDefinitionId");

            snap.Property(p => p.ResultInstance)
                .HasColumnName("ResInstance")
                .HasMaxLength(100)
                .IsRequired();

            snap.Property(p => p.Unit)
                .HasColumnName("ResUnit")
                .HasMaxLength(50)
                .IsRequired();

            snap.Property(p => p.MinValue)
                .HasColumnName("ResSpecMin")
                .HasColumnType("decimal(18,6)");

            snap.Property(p => p.MaxValue)
                .HasColumnName("ResSpecMax")
                .HasColumnType("decimal(18,6)");
        });
    }
}
