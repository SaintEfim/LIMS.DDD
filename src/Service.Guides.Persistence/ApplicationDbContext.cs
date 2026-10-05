using Service.Guides.Domain;
using Microsoft.EntityFrameworkCore;

namespace Service.Guides.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Unit> Units { get; set; }
    public DbSet<Service.Guides.Domain.Sites.Site> Sites { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
