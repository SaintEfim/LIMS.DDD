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
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        var parameter = studyTemplate?.InputParameters.SingleOrDefault(p => p.Id == new InputParameterId(parameterId));

        return parameter != null ? InputParameterDto.FromDomain(parameter) : null;
    }

    public async Task<ICollection<InputParameterDto>> GetAllByTemplateIdAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return [];
        }

        return studyTemplate.InputParameters
            .Select(InputParameterDto.FromDomain)
            .ToList();
    }
}
