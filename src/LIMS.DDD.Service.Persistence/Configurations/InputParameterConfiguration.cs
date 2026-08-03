using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class InputParameterConfiguration : IEntityTypeConfiguration<InputParameter>
{
    public void Configure(
        EntityTypeBuilder<InputParameter> builder)
    {
        builder.ToTable("InputParameters");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, id => new InputParameterId(id));

        builder.Property(x => x.StudyTemplateId)
            .HasConversion(id => id.Value, id => new StudyTemplateId(id));

        builder.Property(x => x.Name)
            .HasConversion(n => n.Value, n => Name.Create(n)
                .GetValue())
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(d => d.Value, d => Description.Create(d)
                .GetValue())
            .HasMaxLength(1000);

        builder.Property(x => x.AliasName)
            .HasConversion(a => a.Value, a => AliasName.Create(a)
                .GetValue())
            .HasMaxLength(100);

        builder.OwnsOne(x => x.Specification, vr =>
        {
            vr.Property(p => p.MaxValue);
            vr.Property(p => p.MinValue);
        });

        builder.HasIndex(x => new
            {
                x.StudyTemplateId,
                x.AliasName
            })
            .IsUnique();
    }
}
