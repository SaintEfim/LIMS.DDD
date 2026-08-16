using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;

public interface IStudyTemplateRepository : IRepository<StudyTemplate>
{
    Task<ICollection<StudyTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<StudyTemplate?> GetByIdAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default);

    Task<ResultDefinition?> GetResultDefinitionAsync(
        StudyTemplateId studyTemplateId,
        ResultDefinitionId requiredResultDefinitionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResultDefinition>> GetResultDefinitionsAsync(
        StudyTemplateId studyTemplateId,
        CancellationToken cancellationToken = default);

    Task<StudyTemplate?> GetByIdForChangeAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default);

    void Add(
        StudyTemplate studyTemplate);
}
