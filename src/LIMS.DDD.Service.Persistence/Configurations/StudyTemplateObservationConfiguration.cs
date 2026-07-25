using LIMS.DDD.Service.Domain;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateObservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

public class StudyTemplateObservationConfiguration : IEntityTypeConfiguration<StudyTemplateObservation>
{
    public void Configure(
        EntityTypeBuilder<StudyTemplateObservation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, id => new StudyTemplateObservationId(id));

        builder.Property(x => x.StudyTemplateId)
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

        builder.Property(x => x.AliasName)
            .HasConversion(a => a.Value, a => AliasName.Create(a)
                .Value)
            .HasMaxLength(100);

        builder.OwnsOne(x => x.Specification, vr =>
        {
            vr.Property(p => p.MaxValue);
            vr.Property(p => p.MinValue);
        });
    }
}
