using LIMS.DDD.Service.Domain;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class StudyTemplateConfiguration : IEntityTypeConfiguration<StudyTemplate>
{
    public void Configure(
        EntityTypeBuilder<StudyTemplate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, id => new StudyTemplateId(id));

        builder.Property(x => x.Name)
            .HasConversion(n => n.Value, n => Name.Create(n)
                .Value)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(d => d.Value, d => Description.Create(d)
                .Value)
            .HasMaxLength(1000);

        builder.Property(x => x.Revision)
            .HasConversion(r => r.Value, r => Revision.Create(r)
                .Value)
            .IsRequired();

        builder.HasMany(x => x.InputParameters)
            .WithOne()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ResultDefinitions)
            .WithOne()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.CalculationRules)
            .WithOne()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ParentId)
            .HasConversion(id => id.HasValue ? id.Value.Value : (Guid?) null,
                id => id.HasValue ? new StudyTemplateId(id.Value) : null)
            .IsRequired(false);

        builder.HasIndex(x => new
            {
                x.Name,
                x.Revision
            })
            .IsUnique();
    }
}
