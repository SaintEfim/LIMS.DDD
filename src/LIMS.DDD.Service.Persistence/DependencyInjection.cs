using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.DDD.Service.Persistence;

public static class DependencyInjection
{
    public static void AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IStudyTemplateRepository, StudyTemplateRepository>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ServiceDB")));
    }
}
