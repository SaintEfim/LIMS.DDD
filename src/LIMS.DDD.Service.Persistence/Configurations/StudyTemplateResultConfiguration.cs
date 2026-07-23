using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class StudyTemplateResultConfiguration : IEntityTypeConfiguration<StudyTemplateResult>
{
    public void Configure(
        EntityTypeBuilder<StudyTemplateResult> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, id => new StudyTemplateResultId(id));

        builder.Property(x => x.StudyTemplateId)
            .HasConversion(id => id.Value, id => new StudyTemplateId(id));

        builder.Property(x => x.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(x => x.ValueRange, vr =>
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
