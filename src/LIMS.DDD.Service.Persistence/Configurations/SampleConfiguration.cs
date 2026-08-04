using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.DDD.Service.Persistence.Configurations;

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
            .OnDelete(DeleteBehavior.Cascade);

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
                .HasColumnName("GatherDateBegin")
                .HasColumnType("datetimeoffset");

            gd.Property(p => p.End)
                .HasColumnName("GatherDateEnd")
                .HasColumnType("datetimeoffset");
        });

        builder.OwnsOne(x => x.Volume, v =>
        {
            v.Property(p => p.Value)
                .HasColumnName("VolumeValue")
                .HasColumnType("decimal(18,4)");
            v.Property(p => p.Unit)
                .HasColumnName("VolumeUnit")
                .HasMaxLength(50);
        });

        builder.Property(x => x.SampleStatus)
            .HasConversion(status => status.Name, statusName => SampleStatus.ConvertStatus(statusName))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
