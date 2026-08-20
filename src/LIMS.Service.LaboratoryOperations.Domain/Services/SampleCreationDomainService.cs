using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

public sealed class SampleCreationDomainService
{
    public Result<Sample, Exception> CreateSample(
        Order order,
        Name name,
        GatherDate gatherDate,
        Code code,
        Volume volume)
    {
        if (!order.CanAcceptNewEntity)
        {
            return new InvalidOperationException(
                $"Cannot add samples to an order with status '{order.OrderStatus.Name}'. " +
                "Order must be in Draft or InProgress status.");
        }

        var sampleResult = new Sample(order.Id, name, gatherDate, code, volume);

        return sampleResult;
    }
}
