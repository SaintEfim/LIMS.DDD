using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Services;

public sealed class StudyCreationDomainService
{
    public Result<Study, Exception> Create(
        Sample sample,
        Order order,
        StudyTemplateSnapshot templateSnapshot)
    {
        if (!order.CanAcceptNewEntity)
        {
            return Result<Study, Exception>.Failure(
                new InvalidOperationException(
                    $"Cannot create study for an order in '{order.OrderStatus.Name}' status."));
        }

        if (sample.SampleStatus == SampleStatus.Canceled || sample.SampleStatus == SampleStatus.Completed)
        {
            return Result<Study, Exception>.Failure(
                new InvalidOperationException(
                    $"Cannot create study for a sample in '{sample.SampleStatus.Name}' status."));
        }

        var studyId = new StudyId(Guid.NewGuid());

        var measuredValues = templateSnapshot.Parameters
            .Select(p => MeasuredValue.Create(studyId, p, null))
            .ToList();

        var testResults = templateSnapshot.Results
            .Select(r => TestResult.Create(studyId, r, null, false))
            .ToList();

        return Study.Create(studyId, sample.Id, templateSnapshot, measuredValues, testResults);
    }
}
