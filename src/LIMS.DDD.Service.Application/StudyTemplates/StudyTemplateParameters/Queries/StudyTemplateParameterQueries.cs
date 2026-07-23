using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateParameters;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Queries;

public sealed class StudyTemplateParameterQueries(IStudyTemplateRepository repository)
{
    public async Task<StudyTemplateParameterDto?> GetByIdAsync(
        Guid studyTemplateId,
        Guid parameterId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(
            new StudyTemplateId(studyTemplateId),
            cancellationToken);

        var parameter = studyTemplate.Parameters
            .SingleOrDefault(p => p.Id == new StudyTemplateParameterId(parameterId));

        return parameter != null
            ? StudyTemplateParameterDto.FromDomain(parameter)
            : null;
    }

    public async Task<ICollection<StudyTemplateParameterDto>> GetAllByTemplateIdAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(
            new StudyTemplateId(studyTemplateId),
            cancellationToken);

        return studyTemplate.Parameters
            .Select(StudyTemplateParameterDto.FromDomain)
            .ToList();
    }
}
