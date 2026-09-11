using Guides.Service.Domain;
using Library.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Guides.Service.Persistence;

public static class DependencyInjection
{
    public static void AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUnitRepository, UnitRepository>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ServiceDB")));

        services.AddScoped<DbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
}
