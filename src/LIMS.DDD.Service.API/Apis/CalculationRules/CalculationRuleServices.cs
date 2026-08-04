using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules;
using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;

namespace LIMS.DDD.Service.API.Apis.CalculationRules;

public class CalculationRuleServices(
    CalculationRuleCommandHandler commands,
    CalculationRuleQueries queries)
{
    public CalculationRuleCommandHandler Commands { get; } = commands;
    public CalculationRuleQueries Queries { get; } = queries;
}
