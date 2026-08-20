using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.SoftDeletable;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;

public class Order
    : SoftDeletableModel,
        IAggregateRoot
{
    public Order(
        Name name,
        Description description,
        string contractor,
        Code code)
    {
        Id = new OrderId(Guid.NewGuid());
        Name = name;
        Description = description;
        Contractor = contractor;
        Code = code;
    }

    // for EF Core
    private Order()
    {
    }

    public OrderId Id { get; private set; }

    public Name Name { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public Code Code { get; private set; } = null!;

    public string Contractor { get; private set; } = null!;

    public OrderStatus OrderStatus { get; private set; } = OrderStatus.Draft;

    public bool CanAcceptNewEntity => OrderStatus == OrderStatus.Draft || OrderStatus == OrderStatus.InProgress;

    public bool CanDeleteAssociatedEntities => OrderStatus == OrderStatus.Draft;

    public Result<None, Exception> Delete()
    {
        if (OrderStatus != OrderStatus.Draft)
        {
            return new InvalidOperationException($"Cannot delete order in '{OrderStatus.Name}' status. " +
                                                 "Only orders in 'Draft' status can be deleted. Use 'Cancel' status for others.");
        }

        if (IsDeleted)
        {
            return new InvalidOperationException("Order is already deleted.");
        }

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;

        return new None();
    }

    public Result<None, Exception> UpdatePartial(
        Name? name,
        Description? description,
        string? contractor,
        Code? code)
    {
        if (!OrderStatus.CanEdit)
        {
            return new InvalidOperationException("Cannot modify order details when the order is not editable.");
        }

        if (name is not null)
        {
            Name = name;
        }

        if (description is not null)
        {
            Description = description;
        }

        if (contractor is not null)
        {
            Contractor = contractor;
        }

        if (code is not null)
        {
            Code = code;
        }

        return new None();
    }

    internal Result<None, Exception> ChangeStatus(
        OrderStatus newOrderStatus)
    {
        var result = OrderStatus.CanTransitionTo(newOrderStatus, this);

        if (result.IsFailure)
        {
            return result.CastFailure<None>();
        }

        OrderStatus = newOrderStatus;

        return new None();
    }
}
