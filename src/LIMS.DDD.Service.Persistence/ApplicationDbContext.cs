using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.DDD.Service.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public DbSet<StudyTemplate> StudyTemplates { get; set; }
}
