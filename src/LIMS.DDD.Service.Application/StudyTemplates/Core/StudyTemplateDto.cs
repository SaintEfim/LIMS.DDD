using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.InputParameters;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions;
using LIMS.DDD.Service.Domain.SeedWork.Snapshots;
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
        StudyTemplate template,
        IReadOnlyDictionary<UnitId, UnitSnapshot> units)
    {
        return new StudyTemplateDto(template.ParentId?.Value, template.Id.Value, template.Name.Value,
            template.Description.Value, template.Revision.Value, template.Status.Name,
            template.ResultDefinitions.Select(r =>
                ResultDefinitionDto.FromDomain(units.GetValueOrDefault(r.UnitId), r)),
            template.InputParameters.Select(InputParameterDto.FromDomain),
            template.CalculationRules.Select(CalculationRuleDto.FromDomain));
    }
}
