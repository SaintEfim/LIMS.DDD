using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

public sealed class SampleDeletionDomainService
{
    public Result<None, Exception> DeleteSample(
        Sample sample,
        Order order,
        bool hasAssociatedStudies)
    {
        if (!order.CanDeleteAssociatedEntities)
        {
            return Result<None, Exception>.Failure(new InvalidOperationException(
                $"Cannot delete sample from an order with status '{order.OrderStatus.Name}'. " +
                "Order must be in Draft status."));
        }

        if (hasAssociatedStudies)
        {
            return Result<None, Exception>.Failure(new InvalidOperationException(
                "Cannot delete sample because it has associated study(ies). " +
                "Please cancel or delete the studies first."));
        }

        if (sample.SampleStatus != SampleStatus.Registered)
        {
            return Result<None, Exception>.Failure(new InvalidOperationException(
                $"Cannot delete sample in '{sample.SampleStatus.Name}' status. Only 'Registered' samples can be deleted."));
        }

        return sample.Delete();
    }
}
