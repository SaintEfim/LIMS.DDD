using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

public sealed class SampleCreationDomainService
{
    public Result<Sample, DomainError> CreateSample(
        Order order,
        Name name,
        GatherDate gatherDate,
        Code code,
        Volume volume)
    {
        if (!order.CanAcceptNewEntity)
        {
            return new EntityNotEditableError(nameof(Order), order.OrderStatus.Name, "accept new samples");
        }

        var sample = new Sample(order.Id, name, gatherDate, code, volume);

        return sample;
    }
}
