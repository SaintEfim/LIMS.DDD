using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules;

namespace LIMS.DDD.Service.API.Apis.CalculationRules;

public class CalculationRuleServices(CalculationRuleCommandsHandler commands, CalculationRuleQueries queries)
{
    public CalculationRuleCommandsHandler Commands { get; } = commands;
    public CalculationRuleQueries Queries { get; } = queries;
}
