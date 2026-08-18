using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

public sealed class StudyCreationDomainService
{
    public Result<Study, Exception> CreateStudyByTemplate(
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

        if (!sample.CanAcceptNewEntity)
        {
            return Result<Study, Exception>.Failure(
                new InvalidOperationException(
                    $"Cannot create study for a sample in '{sample.SampleStatus.Name}' status."));
        }

        var studyId = new StudyId(Guid.NewGuid());

        var measuredValues = templateSnapshot.Parameters
            .Select(p => MeasuredValue.Create(studyId, p.Id))
            .ToList();

        var testResults = templateSnapshot.Results
            .Select(r => TestResult.Create(studyId, r.Id))
            .ToList();

        return Study.Create(studyId, sample.Id, templateSnapshot.Name, templateSnapshot.Id, measuredValues,
            testResults);
    }
}
