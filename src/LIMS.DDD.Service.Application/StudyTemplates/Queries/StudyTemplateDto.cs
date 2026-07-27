using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Queries;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates.Queries;

public sealed record StudyTemplateDto(
    Guid Id,
    string Name,
    string? Description,
    string Revision,
    string Status,
    IEnumerable<ResultDefinitionDto> ResultDefinitions,
    IEnumerable<InputParameterDto> InputParameters,
    IEnumerable<CalculationRuleDto> CalculationRules)
{
    public static StudyTemplateDto FromDomain(
        StudyTemplate template)
    {
        return new StudyTemplateDto(Id: template.Id.Value, Name: template.Name, Description: template.Description,
            Revision: template.Revision, Status: template.Status.ToString(),
            ResultDefinitions: template.ResultDefinitions.Select(ResultDefinitionDto.FromDomain),
            InputParameters: template.InputParameters.Select(InputParameterDto.FromDomain),
            CalculationRules: template.CalculationRules.Select(CalculationRuleDto.FromDomain));
    }
}
