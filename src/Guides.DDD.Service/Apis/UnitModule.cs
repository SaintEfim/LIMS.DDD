using Carter;
using Guides.DDD.Service.Commands;
using Guides.DDD.Service.Domains;
using Guides.DDD.Service.Persistence;
using Guides.Messages;
using Microsoft.EntityFrameworkCore;
using RabbitMq.Library.QuickStart.Abstractions;

namespace Guides.DDD.Service.Apis;

public class UnitModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/units")
            .WithTags("Units");

        group.MapGet("/", async (
            ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            var units = await db.Units
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Results.Ok(units);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            var unit = await db.Units
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            return unit is null ? Results.NotFound() : Results.Ok(unit);
        });

        group.MapPost("/", async (
            CreateUnitCommand unitCommand,
            IMessageBus busService,
            ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            var unit = new Unit { Name = unitCommand.Name };

            db.Units.Add(unit);

            await db.SaveChangesAsync(cancellationToken);

            await busService.SendAsync(new UnitCreated(unit.Name), cancellationToken);

            return Results.Created($"/api/units/{unit.Id}", unit);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            Unit updatedUnit,
            ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            if (id != updatedUnit.Id) return Results.BadRequest();

            var existingUnit = await db.Units.FindAsync([id], cancellationToken);
            if (existingUnit is null) return Results.NotFound();

            existingUnit.Name = updatedUnit.Name;
            await db.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            var unit = await db.Units.FindAsync([id], cancellationToken);

            if (unit is null) return Results.NotFound();

            unit.IsDeleted = true;
            await db.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });
    }
}
