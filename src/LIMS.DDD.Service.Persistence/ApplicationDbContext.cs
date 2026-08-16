using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Snapshots;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.DDD.Service.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options),
    IUnitOfWork
{
    public DbSet<StudyTemplate> StudyTemplates { get; set; }

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
