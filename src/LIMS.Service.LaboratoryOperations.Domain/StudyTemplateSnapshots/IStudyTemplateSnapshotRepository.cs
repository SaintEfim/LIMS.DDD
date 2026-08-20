using Domain.SeedWork.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

public interface IStudyTemplateSnapshotRepository : IRepository<StudyTemplateSnapshot>
{
    Task<ICollection<StudyTemplateSnapshot>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<StudyTemplateSnapshot?> GetByIdAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default);

    Task<ResultDefinitionSnapshot?> GetResultDefinitionAsync(
        StudyTemplateId studyTemplateId,
        ResultDefinitionId requiredResultDefinitionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResultDefinitionSnapshot>> GetResultDefinitionsAsync(
        StudyTemplateId studyTemplateId,
        CancellationToken cancellationToken = default);

    Task<InputParameterSnapshot?> GetInputParameterAsync(
        StudyTemplateId studyTemplateId,
        InputParameterId requiredResultDefinitionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InputParameterSnapshot>> GetInputParameterSnapshotsAsync(
        StudyTemplateId studyTemplateId,
        CancellationToken cancellationToken = default);

    Task<StudyTemplateSnapshot?> GetByIdForChangeAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default);

    void Add(
        StudyTemplateSnapshot id);
}
