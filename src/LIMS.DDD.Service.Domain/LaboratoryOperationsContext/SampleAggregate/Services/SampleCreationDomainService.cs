using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.Services;

public sealed class SampleCreationDomainService
{
    public Result<Sample, Exception> CreateSample(
        Order order,
        Name name,
        GatherDate gatherDate,
        Code code,
        Volume volume)
    {
        if (!order.CanAcceptNewSamples)
        {
            return Result<Sample, Exception>.Failure(new InvalidOperationException(
                $"Cannot add samples to an order with status '{order.OrderStatus.Name}'. " +
                "Order must be in Draft or InWork status."));
        }

        var sampleResult = Sample.Create(order.Id, name, gatherDate, code, volume);

        return sampleResult;
    }
}
