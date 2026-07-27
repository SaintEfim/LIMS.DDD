using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ResultDefinitions;

namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Queries;

public sealed class ResultDefinitionQueries(IStudyTemplateRepository repository)
{
    public async Task<ResultDefinitionDto?> GetByIdAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        var result =
            studyTemplate?.ResultDefinitions.SingleOrDefault(r => r.Id == new ResultDefinitionId(resultId));

        return result != null ? ResultDefinitionDto.FromDomain(result) : null;
    }

    public async Task<ICollection<ResultDefinitionDto>> GetAllByTemplateIdAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null) return [];

        return studyTemplate.ResultDefinitions
            .Select(ResultDefinitionDto.FromDomain)
            .ToList();
    }
}
