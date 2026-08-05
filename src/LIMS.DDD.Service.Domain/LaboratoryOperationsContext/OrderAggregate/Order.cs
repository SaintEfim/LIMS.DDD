using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;

public class Order
    : SoftDeletableModel,
        IAggregateRoot
{
    public OrderId Id { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public Code Code { get; private set; }

    public string Contractor { get; private set; } = string.Empty;

    public OrderStatus OrderStatus { get; private set; } = OrderStatus.Draft;

    public bool CanAcceptNewSamples => OrderStatus == OrderStatus.Draft || OrderStatus == OrderStatus.InProgress;

    public bool CanDeleteAssociatedEntities => OrderStatus == OrderStatus.Draft;

    private Order()
    {
    }

    public static Result<Order, Exception> Create(
        Name name,
        Description description,
        string contractor)
    {
        var order = new Order
        {
            Id = new OrderId(Guid.NewGuid()),
            Name = name,
            Description = description,
            Contractor = contractor
        };

        return Result<Order, Exception>.Success(order);
    }

    public Result<Exception> Delete()
    {
        if (OrderStatus != OrderStatus.Draft)
            return Result<Exception>.Failure(new InvalidOperationException(
                $"Cannot delete order in '{OrderStatus.Name}' status. " +
                "Only orders in 'Draft' status can be deleted. Use 'Cancel' status for others."));

        if (IsDeleted) return Result<Exception>.Failure(new InvalidOperationException("Sample is already deleted."));

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;

        return Result<Exception>.Success();
    }

    public Result<Exception> UpdatePartial(
        Name? name,
        Description? description,
        string? contractor)
    {
        if (!OrderStatus.CanEdit)
            return Result<Exception>.Failure(
                new InvalidOperationException(
                    "Cannot modify details of an Active or Archived template. Create a new revision."));

        if (name is not null) Name = name;
        if (description is not null) Description = description;
        if (contractor is not null) Contractor = contractor;

        return Result<Exception>.Success();
    }

    public Result<Exception> ChangeStatus(
        OrderStatus newOrderStatus)
    {
        var result = OrderStatus.CanTransitionTo(newOrderStatus, this);

        if (result.IsFailure) return result;

        OrderStatus = newOrderStatus;

        return Result<Exception>.Success();
    }
}
