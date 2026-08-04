using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.InputParameters;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates.Core;

public sealed record StudyTemplateDto(
    Guid? ParentId,
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
        return new StudyTemplateDto(ParentId: template.ParentId?.Value, Id: template.Id.Value, Name: template.Name.Value,
            Description: template.Description.Value, Revision: template.Revision.Value,
            Status: template.Status.Name,
            ResultDefinitions: template.ResultDefinitions.Select(ResultDefinitionDto.FromDomain),
            InputParameters: template.InputParameters.Select(InputParameterDto.FromDomain),
            CalculationRules: template.CalculationRules.Select(CalculationRuleDto.FromDomain));
    }
}
