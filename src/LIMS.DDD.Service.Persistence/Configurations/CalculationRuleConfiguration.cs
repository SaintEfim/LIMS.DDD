using LIMS.DDD.Service.Domain;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ResultDefinitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class CalculationRuleConfiguration : IEntityTypeConfiguration<CalculationRule>
{
    public void Configure(
        EntityTypeBuilder<CalculationRule> builder)
    {
        builder.ToTable("CalculationRules");

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
                .Value)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FormulaExpression)
            .HasConversion(n => n.Value, n => FormulaExpression.Create(n)
                .Value)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(d => d.Value, d => Description.Create(d)
                .Value)
            .HasMaxLength(1000);

        builder.OwnsMany(x => x.CalculationInputs, inputBuilder =>
        {
            inputBuilder.HasIndex(x => new
                {
                    x.ParameterId,
                    x.VariableAlias
                })
                .IsUnique();

            inputBuilder.ToTable("CalculationInputs");

            inputBuilder.Property(x => x.Id)
                .HasConversion(id => id.Value, id => new CalculationInputId(id));

            inputBuilder.WithOwner()
                .HasForeignKey("CalculationRuleId");

            inputBuilder.Property(x => x.VariableAlias)
                .HasConversion(a => a.Value, a => AliasName.Create(a)
                    .Value)
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
