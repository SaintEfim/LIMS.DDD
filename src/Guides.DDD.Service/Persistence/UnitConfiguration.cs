using Guides.DDD.Service.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guides.DDD.Service.Persistence;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(
        EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
