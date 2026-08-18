using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.TestResults;

public sealed class TestResultQueries(IStudyRepository repository, IStudyTemplateSnapshotRepository snapshotRepository)
{
    public async Task<TestResultDto?> GetByIdAsync(
        Guid studyId,
        Guid testResultId,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(studyId), cancellationToken);
        if (study is null) throw new KeyNotFoundException("study not found");

        var testResult = study.TestResults.SingleOrDefault(tr => tr.Id == new TestResultId(testResultId));
        if (testResult is null) return null;

        var resultDefinition = await snapshotRepository.GetResultDefinitionAsync(study.StudyTemplateId,
            testResult.ResultDefinitionId, cancellationToken);
        if (resultDefinition is null) throw new KeyNotFoundException("result definition not found");

        return TestResultDto.FromDomain(testResult, resultDefinition);
    }

    public async Task<ICollection<TestResultDto>> GetAllByStudyIdAsync(
        Guid studyId,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(studyId), cancellationToken);
        if (study is null) return [];

        var snapshot = await snapshotRepository.GetByIdAsync(study.StudyTemplateId, cancellationToken);
        if (snapshot is null) throw new KeyNotFoundException("template not found");

        var resultDefinitions =
            await snapshotRepository.GetResultDefinitionsAsync(study.StudyTemplateId, cancellationToken);

        var resultDefinitionsDict = resultDefinitions.ToDictionary(rd => rd.Id);

        return study.TestResults
            .Select(tr =>
            {
                var resultDefinition = resultDefinitionsDict.GetValueOrDefault(tr.ResultDefinitionId);
                return TestResultDto.FromDomain(tr,
                    resultDefinition ?? throw new KeyNotFoundException("result definition not found"));
            })
            .ToList();
    }
}
