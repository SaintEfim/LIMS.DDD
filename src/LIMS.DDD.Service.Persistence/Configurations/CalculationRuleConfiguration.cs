using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class CalculationRuleConfiguration : IEntityTypeConfiguration<CalculationRule>
{
    public void Configure(
        EntityTypeBuilder<CalculationRule> builder)
    {
        builder.ToTable("CalculationRules");

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
            .HasConversion(n => n.Value, n => FormulaExpression.Create(n)
                .GetValue())
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(d => d.Value, d => Description.Create(d)
                .GetValue())
            .HasMaxLength(1000);

        builder.OwnsMany(x => x.CalculationInputs, inputBuilder =>
        {
            inputBuilder.ToJson();

            inputBuilder.Property(x => x.VariableAlias)
                .HasConversion(a => a.Value, a => AliasName.Create(a)
                    .GetValue())
                .HasMaxLength(100);

            inputBuilder.Property(x => x.ParameterId)
                .HasConversion(id => id.Value, id => new InputParameterId(id))
                .IsRequired();
        });

        builder.HasIndex(x => new
            {
                x.StudyTemplateId,
                x.Name
            })
            .IsUnique();
    }
}
