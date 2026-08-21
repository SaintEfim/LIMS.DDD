using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.LaboratoryOperations.Persistence.Configurations;

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

        builder.Property(x => x.ResultDefinitionId)
            .HasConversion(id => id.Value, v => new ResultDefinitionId(v))
            .IsRequired();

        builder.HasOne<ResultDefinitionSnapshot>()
            .WithMany()
            .HasForeignKey(x => x.ResultDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
