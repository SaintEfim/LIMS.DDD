using Library.Domain.SeedWork.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.Guides.Domain.Sites;
namespace Service.Guides.Persistence.Configurations;

public sealed class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Sites", table => table.HasCheckConstraint("CK_Sites_Parent_NotSelf", "\"ParentId\" IS NULL OR \"ParentId\" <> \"Id\""));
        builder.HasKey(site => site.Id);
        builder.Property(site => site.Id).HasConversion(id => id.Value, value => new SiteId(value));
        builder.Property(site => site.ParentId).HasConversion(id => id!.Value.Value, value => new SiteId(value));
        builder.Property(site => site.Name).HasConversion(name => name.Value, value => Name.Create(value).GetValue())
            .HasMaxLength(100).IsRequired();
        builder.Property(site => site.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(site => !site.IsDeleted);
        builder.HasOne<Site>().WithMany().HasForeignKey(site => site.ParentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(site => site.ParentId);
    }
}