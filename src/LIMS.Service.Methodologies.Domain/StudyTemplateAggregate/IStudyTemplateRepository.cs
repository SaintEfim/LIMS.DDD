using LIMS.Service.Methodologies.Domain.SeedWork;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;

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

    Task<InputParameter?> GetInputParameterAsync(
        StudyTemplateId studyTemplateId,
        InputParameterId requiredResultDefinitionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InputParameter>> GetInputParameterSnapshotsAsync(
        StudyTemplateId studyTemplateId,
        CancellationToken cancellationToken = default);

    Task<StudyTemplate?> GetByIdForChangeAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default);

    void Add(
        StudyTemplate studyTemplate);
}
