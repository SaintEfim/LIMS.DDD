using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.LaboratoryOperations.Persistence.Configurations;

public class StudyConfiguration : IEntityTypeConfiguration<Study>
{
    public void Configure(
        EntityTypeBuilder<Study> builder)
    {
        builder.ToTable("Studies");

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
        builder.Property(x => x.DeletedAt)
            .IsRequired(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new StudyId(value));

        builder.Property(x => x.StudyTemplateId)
            .HasConversion(id => id.Value, v => new StudyTemplateId(v))
            .IsRequired();

        builder.HasOne<StudyTemplateSnapshot>()
            .WithMany()
            .HasForeignKey(x => x.StudyTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SampleId)
            .HasConversion(id => id.Value, value => new SampleId(value));

        builder.Property(x => x.Name)
            .HasConversion(n => n.Value, n => Name.Create(n)
                .GetValue())
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne<Sample>()
            .WithMany()
            .HasForeignKey(x => x.SampleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Description)
            .HasConversion(d => d.Value, d => Description.Create(d)
                .GetValue())
            .HasMaxLength(2000);

        builder.Property(x => x.Status)
            .HasConversion(status => status.Name, statusName => StudyStatus.ConvertStatus(statusName))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(x => x.MeasuredValues)
            .WithOne()
            .HasForeignKey(x => x.StudyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TestResults)
            .WithOne()
            .HasForeignKey(x => x.StudyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
