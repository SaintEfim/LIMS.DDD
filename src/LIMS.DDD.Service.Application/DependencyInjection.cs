using LIMS.DDD.Service.Application.StudyTemplates.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.DDD.Service.Application;

public static class DependencyInjection
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<StudyTemplateCommands>();
        services.AddScoped<StudyTemplateQueries>();

        services.AddScoped<StudyTemplateObservationCommands>();
        services.AddScoped<StudyTemplateObservationQueries>();

        services.AddScoped<StudyTemplateDeterminationCommands>();
        services.AddScoped<StudyTemplateDeterminationQueries>();
    }
}
