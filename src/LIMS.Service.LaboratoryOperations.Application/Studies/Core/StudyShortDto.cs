using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.Core;

public sealed record StudyShortDto(
    Guid Id,
    Guid SampleId,
    string Status,
    string Name,
    Guid TemplateId,
    string? Description)
{
    public static StudyShortDto FromDomain(
        Study study)
    {
        return new StudyShortDto(study.Id.Value, study.SampleId.Value, study.Status.Name, study.Name.Value,
            study.StudyTemplateId.Value, study.Description.Value);
    }
}
