using Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

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
                return new InvalidOperationException(
                    "Cannot complete the order because there are samples in 'Registered' or 'InProgress' status. " +
                    "Please complete or cancel all samples first.");
            }
        }

        if (newStatus == OrderStatus.Canceled)
        {
            var hasActiveSamples = associatedSamples.Any(s =>
                s.SampleStatus == SampleStatus.Registered || s.SampleStatus == SampleStatus.InProgress);

            if (hasActiveSamples)
            {
                return new InvalidOperationException("Cannot cancel the order because there are active samples. " +
                                                     "Please cancel all samples first.");
            }
        }

        return order.ChangeStatus(newStatus);
    }
}
