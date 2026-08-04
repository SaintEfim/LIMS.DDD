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

        builder.Property(x => x.ResultDefinitionId)
            .HasConversion(id => id.Value, value => new ResultDefinitionId(value));

        builder.Property(x => x.Value)
            .HasColumnType("decimal(18,6)");

        builder.Property(x => x.IsOutOfSpec)
            .IsRequired();
    }
}
