using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LIMS.Service.LaboratoryOperations.Persistence.Configurations;

public sealed class BackgroundOperationConfiguration : IEntityTypeConfiguration<BackgroundOperation>
{
    public void Configure(EntityTypeBuilder<BackgroundOperation> builder)
    {
        builder.ToTable("BackgroundOperations");
        builder.HasKey(operation => operation.Id);

        builder.Property(operation => operation.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(operation => operation.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(operation => operation.Payload)
            .HasConversion(
                payload => payload.ToString(Formatting.None),
                payload => JObject.Parse(payload))
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(operation => operation.Result).HasColumnType("text");
        builder.Property(operation => operation.Error).HasColumnType("text");
        builder.Property(operation => operation.CreatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(operation => operation.StartedAtUtc).HasColumnType("timestamp with time zone");
        builder.Property(operation => operation.CompletedAtUtc).HasColumnType("timestamp with time zone");

        builder.HasIndex(operation => new { operation.Status, operation.CreatedAtUtc });
        builder.HasIndex(operation => new { operation.RequestedByUserId, operation.CreatedAtUtc });
    }
}
