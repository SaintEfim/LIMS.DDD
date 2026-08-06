using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;

namespace LIMS.DDD.Service.Application.Studies.TestResults;

public sealed record TestResultDto(
    Guid Id,
    Guid StudyId,
    Guid ResultDefinitionId,
    string ResultInstance,
    string Unit,
    double? SpecMin,
    double? SpecMax,
    double? Value,
    bool IsOutOfSpec)
{
    public static TestResultDto FromDomain(
        TestResult tr)
    {
        return new TestResultDto(tr.Id.Value, tr.StudyId.Value, tr.ResultSnapshot.ResultDefinitionId,
            tr.ResultSnapshot.ResultInstance, tr.ResultSnapshot.Unit, tr.ResultSnapshot.MinValue,
            tr.ResultSnapshot.MaxValue, tr.Value, tr.IsOutOfSpec);
    }
}
