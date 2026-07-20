using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.API.Dtos;

public sealed record StudyTemplateDto(
    Guid Id,
    string Name,
    string? Description,
    string Revision,
    string Status,
    List<StudyTemplateResultDto> Results,
    List<StudyTemplateParameterDto> Parameters)
{
    public static StudyTemplateDto FromDomain(
        StudyTemplate template)
    {
        return new StudyTemplateDto(Id: template.Id.Value, Name: template.Name.Value,
            Description: template.Description.Value, Revision: template.Revision.Value,
            Status: template.Status.ToString(), Results: template.Results
                .Select(StudyTemplateResultDto.FromDomain)
                .ToList(), Parameters: template.Parameters
                .Select(StudyTemplateParameterDto.FromDomain)
                .ToList());
    }
}
