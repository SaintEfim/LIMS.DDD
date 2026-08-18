using LIMS.Service.LaboratoryOperations.Application.Studies.MeasuredValues;
using LIMS.Service.LaboratoryOperations.Application.Studies.TestResults;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.Core;

public sealed record StudyDto(
    Guid Id,
    Guid SampleId,
    string Status,
    string Name,
    Guid TemplateId,
    string? Description,
    ICollection<MeasuredValueDto> MeasuredValues,
    ICollection<TestResultDto> TestResults)
{
    public static StudyDto FromDomain(
        Study study,
        StudyTemplateSnapshot templateSnapshot)
    {
        var resultsById = templateSnapshot.Results.ToDictionary(r => r.Id);

        var parametersById = templateSnapshot.Parameters.ToDictionary(p => p.Id);

        return new StudyDto(study.Id.Value, study.SampleId.Value, study.Status.Name, study.Name.Value,
            study.StudyTemplateId.Value, study.Description.Value, study.MeasuredValues
                .Select(mv => MeasuredValueDto.FromDomain(mv, parametersById[mv.InputParameterId]))
                .ToList(), study.TestResults
                .Select(tr => TestResultDto.FromDomain(tr, resultsById[tr.ResultDefinitionId]))
                .ToList());
    }
}
