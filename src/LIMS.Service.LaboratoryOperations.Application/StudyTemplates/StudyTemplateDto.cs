using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public sealed record StudyTemplateDto(
    StudyTemplateId Id,
    Revision Revision,
    Name Name,
    IReadOnlyList<InputParameterDto> Parameters,
    IReadOnlyList<ResultDefinitionDto> Results)
{
    public static StudyTemplateDto FromSnapshot(
        StudyTemplateSnapshot snapshot,
        IReadOnlyList<InputParameterDto> parameters,
        IReadOnlyList<ResultDefinitionDto> results)
    {
        return new StudyTemplateDto(snapshot.Id, snapshot.Revision, snapshot.Name, parameters, results);
    }
}
