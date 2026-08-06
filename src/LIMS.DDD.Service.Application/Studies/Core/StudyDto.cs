using LIMS.DDD.Service.Application.Studies.TestResults;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;

namespace LIMS.DDD.Service.Application.Studies.Core;

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
        Study study)
    {
        return new StudyDto(study.Id.Value, study.SampleId.Value, study.Status.Name, study.Name.Value,
            study.TemplateId.Value, study.Description.Value, study.MeasuredValues
                .Select(MeasuredValueDto.FromDomain)
                .ToList(), study.TestResults
                .Select(TestResultDto.FromDomain)
                .ToList());
    }
}
