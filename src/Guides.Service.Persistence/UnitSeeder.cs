using Guides.Service.Domain;
using Library.Domain.SeedWork;
using Library.Domain.SeedWork.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Guides.Service.Persistence;

public sealed class UnitSeeder(ApplicationDbContext context, IUnitOfWork unitOfWork)
{
    private static readonly UnitSeed[] DefaultUnits =
    [
        new("00000000-0000-0000-0000-000000000001", "%"), new("00000000-0000-0000-0000-000000000002", "°C"),
        new("00000000-0000-0000-0000-000000000003", "г"), new("00000000-0000-0000-0000-000000000004", "кг"),
        new("00000000-0000-0000-0000-000000000005", "л"), new("00000000-0000-0000-0000-000000000006", "мг"),
        new("00000000-0000-0000-0000-000000000007", "мл"), new("00000000-0000-0000-0000-000000000008", "pH"),
        new("00000000-0000-0000-0000-000000000009", "шт.")
    ];

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var existingUnits = await context.Units
            .AsNoTracking()
            .Select(unit => new
            {
                unit.Id,
                Name = unit.Name.Value
            })
            .ToListAsync(cancellationToken);

        var unitsToCreate = DefaultUnits.Where(seed => existingUnits.All(unit =>
                unit.Id != seed.Id && !string.Equals(unit.Name, seed.Name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (unitsToCreate.Count == 0) return;

        foreach (var seed in unitsToCreate)
        {
            var nameResult = Name.Create(seed.Name);
            if (nameResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"The configured unit '{seed.Name}' is invalid: {nameResult.GetError().Message}");
            }

            context.Units.Add(Unit.CreateSystem(seed.Id, nameResult.GetValue()));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private sealed class UnitSeed
    {
        public UnitSeed(
            string id,
            string name)
        {
            Id = new UnitId(Guid.Parse(id));
            Name = name;
        }

        public UnitId Id { get; }

        public string Name { get; }
    }
}
