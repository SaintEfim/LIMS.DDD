using Carter;
using Broker.Messages;
using Guides.Service.Commands;
using Guides.Service.Domains;
using Guides.Service.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMq.Library.Broker;
using RabbitMq.Library.Outbox;

namespace Guides.Service.Apis;

public class UnitModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/units")
            .WithTags("Units");

        group.MapGet("/", async (
            [FromServices] ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            var units = await db.Units
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Results.Ok(units);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            var unit = await db.Units
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            return unit is null ? Results.NotFound() : Results.Ok(unit);
        });

        group.MapPost("/", async (
            CreateUnitCommand unitCommand,
            [FromServices] ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            var unit = new Unit { Name = unitCommand.Name };

            db.Units.Add(unit);

            var message = new UnitCreatedMessage(unit.Id, unit.Name);

            db.InsertOutboxMessage(message);

            await db.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/units/{unit.Id}", unit);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            Unit updatedUnit,
            [FromServices] ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            if (id != updatedUnit.Id)
            {
                return Results.BadRequest();
            }

            var existingUnit = await db.Units.FindAsync([id], cancellationToken);
            if (existingUnit is null)
            {
                return Results.NotFound();
            }

            existingUnit.Name = updatedUnit.Name;
            await db.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] ApplicationDbContext db,
            CancellationToken cancellationToken = default) =>
        {
            var unit = await db.Units.FindAsync([id], cancellationToken);

            if (unit is null)
            {
                return Results.NotFound();
            }

            unit.IsDeleted = true;
            await db.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });
    }
}
