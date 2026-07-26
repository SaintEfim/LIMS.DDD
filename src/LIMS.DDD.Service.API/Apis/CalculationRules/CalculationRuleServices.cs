using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;

namespace LIMS.DDD.Service.API.Apis.CalculationRules;

public class CalculationRuleServices(
    CalculationRuleCommands commands,
    CalculationRuleQueries queries)
{
    public CalculationRuleCommands Commands { get; } = commands;
    public CalculationRuleQueries Queries { get; } = queries;
}
