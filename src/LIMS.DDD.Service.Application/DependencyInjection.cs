using LIMS.DDD.Service.Application.StudyTemplates;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.DDD.Service.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<StudyTemplateCommands>();
        services.AddScoped<StudyTemplateQueries>();

        return services;
    }
}
