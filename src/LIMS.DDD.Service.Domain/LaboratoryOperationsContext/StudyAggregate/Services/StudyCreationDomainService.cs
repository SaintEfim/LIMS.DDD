using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Services;

public sealed class StudyCreationDomainService
{
    public Result<Study, Exception> Create(
        Sample sample,
        StudyTemplateCreateSnapshot templateSnapshot)
    {
        if (!templateSnapshot.CanCreateStudy)
        {
            return Result<Study, Exception>.Failure(
                new InvalidOperationException("Cannot create study from the selected study template."));
        }

        if (!sample.CanAcceptNewEntity)
        {
            return Result<Study, Exception>.Failure(
                new InvalidOperationException(
                    $"Cannot create study for a sample in '{sample.SampleStatus.Name}' status."));
        }

        var studyId = new StudyId(Guid.NewGuid());

        var measuredValues = templateSnapshot.Parameters
            .Select(p => MeasuredValue.Create(studyId, p))
            .ToList();

        var testResults = templateSnapshot.Results
            .Select(r => TestResult.Create(studyId, r))
            .ToList();

        return Study.Create(studyId, sample.Id, templateSnapshot.Name, templateSnapshot.TemplateId, measuredValues,
            testResults);
    }
}
