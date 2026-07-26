using LIMS.DDD.Service.Domain;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class StudyTemplateCalculationConfiguration : IEntityTypeConfiguration<StudyTemplateCalculations>
{
    public void Configure(
        EntityTypeBuilder<StudyTemplateCalculations> builder)
    {
        builder.ToTable("StudyTemplateCalculations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, id => new CalculationRuleId(id));

        builder.Property(x => x.StudyTemplateId)
            .HasConversion(id => id.Value, id => new StudyTemplateId(id))
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

        builder.HasOne<StudyTemplate>()
            .WithMany()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
