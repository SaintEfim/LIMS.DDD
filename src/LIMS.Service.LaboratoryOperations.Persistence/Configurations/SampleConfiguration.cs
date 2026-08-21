using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.LaboratoryOperations.Persistence.Configurations;

public class SampleConfiguration : IEntityTypeConfiguration<Sample>
{
    public void Configure(
        EntityTypeBuilder<Sample> builder)
    {
        builder.ToTable("Samples");

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new SampleId(value));

        builder.Property(x => x.OrderId)
            .HasConversion(id => id.Value, value => new OrderId(value));

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Name)
            .HasConversion(n => n.Value, n => Name.Create(n)
                .GetValue())
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasConversion(c => c.Value, c => Code.Create(c)
                .GetValue())
            .HasMaxLength(100);

        builder.OwnsOne(x => x.GatherDate, gd =>
        {
            gd.Property(p => p.Begin)
                .HasColumnName("GatherDateBegin");

            gd.Property(p => p.End)
                .HasColumnName("GatherDateEnd");
        });

        builder.OwnsOne(x => x.Volume, v =>
        {
            v.Property(p => p.Value)
                .HasColumnName("VolumeValue")
                .HasColumnType("decimal(18,4)");

            v.Property(p => p.UnitId)
                .HasColumnName("VolumeUnitId")
                .HasConversion(id => id.HasValue ? id.Value.Value : (Guid?) null,
                    value => value.HasValue ? new UnitId(value.Value) : null);

            v.HasOne<UnitSnapshot>()
                .WithMany()
                .HasForeignKey(p => p.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Property(x => x.SampleStatus)
            .HasConversion(status => status.Name, statusName => SampleStatus.ConvertStatus(statusName))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
