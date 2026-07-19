using LIMS.DDD.Service.Domain;
using LIMS.DDD.Service.Domain.SeedWork;
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
            .HasConversion(n => n.Value, n => new Name(n))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(d => d.Value, d => new Description(d))
            .HasMaxLength(1000);

        builder.Property(x => x.Revision)
            .HasConversion(r => r.Value, r => new Revision(r))
            .IsRequired();

        builder.HasMany(x => x.Results)
            .WithOne()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Parameters)
            .WithOne()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
            {
                x.Name,
                x.Revision
            })
            .IsUnique();
    }
}
