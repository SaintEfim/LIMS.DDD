using LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Queries;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ResultDefinitions;

namespace LIMS.DDD.Service.Application.StudyTemplates.Queries;

public sealed record StudyTemplateDto(
    Guid Id,
    string Name,
    string? Description,
    string Revision,
    string Status,
    IReadOnlyList<ResultDefinition> ResultDefinitions,
    List<InputParameterDto> InputParameters)
{
    public static StudyTemplateDto FromDomain(
        StudyTemplate template)
    {
        return new StudyTemplateDto(Id: template.Id.Value, Name: template.Name,
            Description: template.Description, Revision: template.Revision,
            Status: template.Status.ToString(), ResultDefinitions: template.ResultDefinitions ,InputParameters: template.InputParameters
                .Select(InputParameterDto.FromDomain)
                .ToList());
    }
}
