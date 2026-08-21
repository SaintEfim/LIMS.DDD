using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.LaboratoryOperations.Persistence.Configurations;

public class StudyTemplateSnapshotConfiguration : IEntityTypeConfiguration<StudyTemplateSnapshot>
{
    public void Configure(
        EntityTypeBuilder<StudyTemplateSnapshot> builder)
    {
        builder.ToTable("StudyTemplateSnapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new StudyTemplateId(value));

        builder.Property(x => x.Name)
            .HasConversion(n => n.Value, n => Name.Create(n)
                .GetValue())
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(d => d.Value, d => Description.Create(d)
                .GetValue())
            .HasMaxLength(2000);

        builder.Property(x => x.Revision)
            .HasConversion(r => r.Value, r => Revision.Create(r)
                .GetValue())
            .IsRequired();

        builder.HasMany(x => x.Parameters)
            .WithOne()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Results)
            .WithOne()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.CalculationRules)
            .WithOne()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
            {
                x.Name,
                x.Revision
            })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
