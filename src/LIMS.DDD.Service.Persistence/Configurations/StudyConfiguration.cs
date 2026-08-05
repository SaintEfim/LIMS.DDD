using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

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

        builder.Property(x => x.SampleId)
            .HasConversion(id => id.Value, value => new SampleId(value));

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

        builder.OwnsOne(x => x.StudyTemplateSnapshot, snap =>
        {
            snap.Property(p => p.TemplateId)
                .HasColumnName("TemplateId");

            snap.Property(p => p.Name)
                .HasConversion(n => n, n => n)
                .HasMaxLength(100)
                .HasColumnName("TemplateName");
        });

        builder.HasMany(x => x.MeasuredValues)
            .WithOne()
            .HasForeignKey(x => x.StudyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TestResults)
            .WithOne()
            .HasForeignKey(x => x.StudyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("SampleId", "TemplateId")
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
