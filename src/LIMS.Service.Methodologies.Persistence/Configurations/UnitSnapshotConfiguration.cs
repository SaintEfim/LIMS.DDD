using Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Domain.UnitSnapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMS.Service.Methodologies.Persistence.Configurations;

public class UnitSnapshotConfiguration : IEntityTypeConfiguration<UnitSnapshot>
{
    public void Configure(
        EntityTypeBuilder<UnitSnapshot> builder)
    {
        builder.ToTable("UnitSnapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
        builder.Property(x => x.DeletedAt)
            .IsRequired(false);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new UnitId(value));

        builder.Property(x => x.Name)
            .HasConversion(n => n.Value, n => Name.Create(n)
                .GetValue())
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
