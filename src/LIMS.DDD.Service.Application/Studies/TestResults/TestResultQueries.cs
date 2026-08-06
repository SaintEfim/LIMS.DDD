using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;

namespace LIMS.DDD.Service.Application.Studies.TestResults;

public sealed class TestResultQueries(IStudyRepository repository)
{
    public async Task<TestResultDto?> GetByIdAsync(
        Guid studyId,
        Guid testResultId,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(studyId), cancellationToken);
        var testResult = study?.TestResults.SingleOrDefault(tr => tr.Id == new TestResultId(testResultId));

        return testResult is not null ? TestResultDto.FromDomain(testResult) : null;
    }

    public async Task<ICollection<TestResultDto>> GetAllByStudyIdAsync(
        Guid studyId,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(studyId), cancellationToken);

        if (study is null)
        {
            return [];
        }

        return study.TestResults
            .Select(TestResultDto.FromDomain)
            .ToList();
    }
}
