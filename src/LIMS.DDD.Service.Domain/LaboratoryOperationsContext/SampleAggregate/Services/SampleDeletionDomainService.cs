using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.Services;

public sealed class SampleDeletionDomainService
{
    public Result<Exception> Delete(
        Sample sample,
        Order order,
        IReadOnlyCollection<Study> studies)
    {
        if (!order.CanDeleteAssociatedEntities)
        {
            return Result<Exception>.Failure(new InvalidOperationException(
                $"Cannot delete sample from an order with status '{order.OrderStatus.Name}'. " +
                "Order must be in Draft or InProgress status."));
        }

        if (studies.Count > 0)
        {
            return Result<Exception>.Failure(new InvalidOperationException(
                $"Cannot delete sample because it has {studies.Count} associated study(ies). " +
                "Please cancel or delete the studies first."));
        }

        if (sample.SampleStatus != SampleStatus.Registered)
        {
            return Result<Exception>.Failure(new InvalidOperationException(
                $"Cannot delete sample in '{sample.SampleStatus.Name}' status. Only 'Registered' samples can be deleted."));
        }

        return sample.Delete();
    }
}
