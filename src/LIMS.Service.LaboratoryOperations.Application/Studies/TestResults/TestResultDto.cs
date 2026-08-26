using LIMS.Service.LaboratoryOperations.Application.StudyTemplates;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.TestResults;

public sealed record TestResultDto(
    Guid Id,
    Guid StudyId,
    ResultDefinitionDto ResultDefinition,
    double? Value,
    bool IsOutOfSpec)
{
    public static TestResultDto FromDomain(
        TestResult tr,
        ResultDefinitionSnapshot templateResult,
        UnitSnapshot? unit)
    {
        return new TestResultDto(tr.Id.Value, tr.StudyId.Value, ResultDefinitionDto.FromSnapshot(unit, templateResult),
            tr.Value, tr.IsOutOfSpec);
    }
}
