using Guides.Service.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Guides.Service.API;

public static class UnitSeedingExtensions
{
    public static async Task SeedUnitsAsync(
        this WebApplication app,
        CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync(cancellationToken);

        var seeder = scope.ServiceProvider.GetRequiredService<UnitSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }
}
