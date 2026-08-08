using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.Services;

public sealed class OrderStatusChangeDomainService
{
    public Result<None, Exception> ValidateAndChangeStatus(
        Order order,
        OrderStatus newStatus,
        IReadOnlyCollection<Sample> associatedSamples)
    {
        if (newStatus == OrderStatus.Completed)
        {
            var hasActiveSamples = associatedSamples.Any(s =>
                s.SampleStatus == SampleStatus.Registered || s.SampleStatus == SampleStatus.InProgress);

            if (hasActiveSamples)
            {
                return Result<None, Exception>.Failure(new InvalidOperationException(
                    "Cannot complete the order because there are samples in 'Registered' or 'InProgress' status. " +
                    "Please complete or cancel all samples first."));
            }
        }

        if (newStatus == OrderStatus.Canceled)
        {
            var hasActiveSamples = associatedSamples.Any(s =>
                s.SampleStatus == SampleStatus.Registered || s.SampleStatus == SampleStatus.InProgress);

            if (hasActiveSamples)
            {
                return Result<None, Exception>.Failure(new InvalidOperationException(
                    "Cannot cancel the order because there are active samples. " + "Please cancel all samples first."));
            }
        }

        return order.ChangeStatus(newStatus);
    }
}
