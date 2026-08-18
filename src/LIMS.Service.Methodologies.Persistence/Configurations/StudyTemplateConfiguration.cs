using LIMS.Service.Methodologies.Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.Methodologies.Persistence.Configurations;

public class StudyTemplateConfiguration : IEntityTypeConfiguration<StudyTemplate>
{
    public void Configure(
        EntityTypeBuilder<StudyTemplate> builder)
    {
        builder.ToTable("StudyTemplates");

        builder.Property(x => x.Id)
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

        builder.Property(x => x.Revision)
            .HasConversion(r => r.Value, r => Revision.Create(r)
                .GetValue())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(status => status.Name, value => Status.ConvertStatus(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

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
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
