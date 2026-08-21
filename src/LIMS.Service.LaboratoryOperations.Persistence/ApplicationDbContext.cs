using Domain.SeedWork.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.LaboratoryOperations.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options),
    IUnitOfWork
{
    public DbSet<StudyTemplateSnapshot> StudyTemplates { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Sample> Samples { get; set; }
    public DbSet<Study> Studies { get; set; }

    public DbSet<UnitSnapshot> UnitSnapshots { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
