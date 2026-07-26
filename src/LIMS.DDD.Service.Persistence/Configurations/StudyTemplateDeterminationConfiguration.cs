using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateDeterminations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class StudyTemplateDeterminationConfiguration : IEntityTypeConfiguration<StudyTemplateDetermination>
{
    public void Configure(
        EntityTypeBuilder<StudyTemplateDetermination> builder)
    {
        builder.ToTable("StudyTemplateDeterminations");

        builder.HasKey(x => x.Id);

         builder.Property(x => x.Id)
            .HasConversion(id => id.Value, id => new StudyTemplateDeterminationId(id));

        builder.Property(x => x.StudyTemplateId)
            .HasConversion(id => id.Value, id => new StudyTemplateId(id));

        builder.Property(x => x.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(x => x.Specification, vr =>
        {
            vr.Property(p => p.MaxValue);
            vr.Property(p => p.MinValue);
        });

        builder.HasIndex(x => new
            {
                x.Unit,
                x.ResultInstance
            })
            .IsUnique();
    }
}
