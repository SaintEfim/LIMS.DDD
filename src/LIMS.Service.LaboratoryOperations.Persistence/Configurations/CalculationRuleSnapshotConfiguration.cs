using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.LaboratoryOperations.Persistence.Configurations;

public class CalculationRuleConfiguration : IEntityTypeConfiguration<CalculationRuleSnapshot>
{
    public void Configure(
        EntityTypeBuilder<CalculationRuleSnapshot> builder)
    {
        builder.ToTable("CalculationRuleCalculationRuleSnapshots");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, id => new CalculationRuleId(id));

        builder.Property(x => x.StudyTemplateId)
            .HasConversion(id => id.Value, id => new StudyTemplateId(id))
            .IsRequired();

        builder.Property(x => x.ResultDefinitionId)
            .HasConversion(id => id.Value, id => new ResultDefinitionId(id))
            .IsRequired();

        builder.Property(x => x.Name)
            .HasConversion(n => n.Value, n => Name.Create(n)
                .GetValue())
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FormulaExpression)
            .HasConversion(f => f.Value, f => FormulaExpression.Create(f)
                .GetValue())
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(d => d.Value, d => Description.Create(d)
                .GetValue())
            .HasMaxLength(1000);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.StudyTemplateId);
    }
}
