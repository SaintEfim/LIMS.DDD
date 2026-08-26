using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.Core;

public sealed record StudyTemplateShortDto(
    Guid? ParentId,
    Guid Id,
    string Name,
    string? Description,
    string Revision,
    string Status)
{
    public static StudyTemplateShortDto FromDomain(
        StudyTemplate template)
    {
        return new StudyTemplateShortDto(template.ParentId?.Value, template.Id.Value, template.Name.Value,
            template.Description.Value, template.Revision.Value, template.Status.Name);
    }
}
