using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

public sealed class SampleDeletionDomainService
{
    public Result<None, DomainError> DeleteSample(
        Sample sample,
        Order order,
        bool hasAssociatedStudies)
    {
        if (!order.CanDeleteAssociatedEntities)
        {
            return new EntityNotEditableError(nameof(Order), order.OrderStatus.Name, "delete associated samples from");
        }

        if (hasAssociatedStudies)
        {
            return new EntityInUseError(nameof(Sample), "associated studies");
        }

        if (sample.SampleStatus != SampleStatus.Registered)
        {
            return new InvalidStatusTransitionError(nameof(Sample), sample.SampleStatus.Name, "Deleted");
        }

        return sample.Delete();
    }
}
