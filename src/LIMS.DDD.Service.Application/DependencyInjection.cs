using LIMS.DDD.Service.Application.StudyTemplates.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.DDD.Service.Application;

public static class DependencyInjection
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<StudyTemplateCommands>();
        services.AddScoped<StudyTemplateQueries>();
        services.AddScoped<StudyTemplateParameterCommands>();
        services.AddScoped<StudyTemplateParameterQueries>();
    }
}
