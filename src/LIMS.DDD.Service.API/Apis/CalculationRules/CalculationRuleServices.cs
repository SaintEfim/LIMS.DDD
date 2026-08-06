using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules;

namespace LIMS.DDD.Service.API.Apis.CalculationRules;

public class CalculationRuleServices(CalculationRuleCommandHandler commands, CalculationRuleQueries queries)
{
    public CalculationRuleCommandHandler Commands { get; } = commands;
    public CalculationRuleQueries Queries { get; } = queries;
}
