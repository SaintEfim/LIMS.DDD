using LIMS.Service.Methodologies.Application.StudyTemplates.CalculationRules;

namespace LIMS.Service.Methodologies.API.Apis.CalculationRules;

public class CalculationRuleServices(CalculationRuleCommandsHandler commands, CalculationRuleQueries queries)
{
    public CalculationRuleCommandsHandler Commands { get; } = commands;
    public CalculationRuleQueries Queries { get; } = queries;
}
