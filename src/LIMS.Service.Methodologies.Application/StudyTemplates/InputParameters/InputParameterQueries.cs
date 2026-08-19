using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.InputParameters;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.InputParameters;

public sealed class InputParameterQueries(IStudyTemplateRepository repository)
{
    public async Task<InputParameterDto?> GetByIdAsync(
        Guid studyTemplateId,
        Guid parameterId,
        CancellationToken cancellationToken = default)
    {
        var parameter = await repository.GetInputParameterAsync(new StudyTemplateId(studyTemplateId),
            new InputParameterId(parameterId), cancellationToken);

        return parameter != null ? InputParameterDto.FromDomain(parameter) : null;
    }

    public async Task<ICollection<InputParameterDto>> GetAllByTemplateIdAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var parameters = await repository.GetInputParameterSnapshotsAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (parameters.Count == 0)
        {
            return [];
        }

        return parameters.Select(InputParameterDto.FromDomain)
            .ToList();
    }
}
