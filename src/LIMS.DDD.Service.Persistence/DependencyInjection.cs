using LIMS.DDD.Service.Domain.StudyTemplate;
using LIMS.DDD.Service.Domain.StudyTemplate.Parameter;
using LIMS.DDD.Service.Domain.StudyTemplate.Result;
using LIMS.DDD.Service.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.DDD.Service.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IStudyTemplateRepository, StudyTemplateRepository>();
        services.AddScoped<IStudyTemplateParameterRepository, StudyTemplateParameterRepository>();
        services.AddScoped<IStudyTemplateResultRepository, StudyTemplateResultRepository>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        return services;
    }
}
