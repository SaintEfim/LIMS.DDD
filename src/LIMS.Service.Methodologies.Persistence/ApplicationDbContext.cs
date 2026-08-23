using Domain.SeedWork;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.UnitSnapshots;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.Methodologies.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options),
    IUnitOfWork
{
    public DbSet<StudyTemplate> StudyTemplates { get; set; }

    public DbSet<UnitSnapshot> UnitSnapshots { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
