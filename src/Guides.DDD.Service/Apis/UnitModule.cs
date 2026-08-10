using Carter;
using Guides.DDD.Service.Domains;
using Guides.DDD.Service.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Guides.DDD.Service.Apis;

public class UnitModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/units")
            .WithTags("Units");

        group.MapGet("/", async (
            ApplicationDbContext db) =>
        {
            var units = await db.Units
                .AsNoTracking()
                .ToListAsync();

            return Results.Ok(units);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            ApplicationDbContext db) =>
        {
            var unit = await db.Units
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            return unit is null ? Results.NotFound() : Results.Ok(unit);
        });

        group.MapPost("/", async (
            Unit unit,
            ApplicationDbContext db) =>
        {
            db.Units.Add(unit);
            await db.SaveChangesAsync();

            return Results.Created($"/api/units/{unit.Id}", unit);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            Unit updatedUnit,
            ApplicationDbContext db) =>
        {
            if (id != updatedUnit.Id) return Results.BadRequest();

            var existingUnit = await db.Units.FindAsync(id);
            if (existingUnit is null) return Results.NotFound();

            existingUnit.Name = updatedUnit.Name;
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ApplicationDbContext db) =>
        {
            var unit = await db.Units.FindAsync(id);
            if (unit is null) return Results.NotFound();

            unit.IsDeleted = true;
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
