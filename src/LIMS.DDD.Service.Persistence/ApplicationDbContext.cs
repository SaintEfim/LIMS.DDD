using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateParameters;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateResults;
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

    public DbSet<StudyTemplateResult> StudyTemplateResults { get; set; }

    public DbSet<StudyTemplateParameter> StudyTemplateParameters { get; set; }
}
