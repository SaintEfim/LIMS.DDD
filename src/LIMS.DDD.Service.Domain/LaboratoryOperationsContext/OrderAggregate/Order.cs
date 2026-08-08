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
    private Order()
    {
    }

    public OrderId Id { get; private set; }

    public Name Name { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public Code Code { get; private set; } = null!;

    public string Contractor { get; private set; } = string.Empty;

    public OrderStatus OrderStatus { get; private set; } = OrderStatus.Draft;

    public bool CanAcceptNewEntity => OrderStatus == OrderStatus.Draft || OrderStatus == OrderStatus.InProgress;

    public bool CanDeleteAssociatedEntities => OrderStatus == OrderStatus.Draft;

    public static Result<Order, Exception> Create(
        Name name,
        Description description,
        string contractor,
        Code code)
    {
        var order = new Order
        {
            Id = new OrderId(Guid.NewGuid()),
            Name = name,
            Description = description,
            Contractor = contractor,
            Code = code
        };

        return Result<Order, Exception>.Success(order);
    }

    public Result<None, Exception> Delete()
    {
        if (OrderStatus != OrderStatus.Draft)
        {
            return Result<None, Exception>.Failure(new InvalidOperationException(
                $"Cannot delete order in '{OrderStatus.Name}' status. " +
                "Only orders in 'Draft' status can be deleted. Use 'Cancel' status for others."));
        }

        if (IsDeleted)
        {
            return Result<None, Exception>.Failure(new InvalidOperationException("Order is already deleted."));
        }

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;

        return Result<None, Exception>.Success();
    }

    public Result<None, Exception> UpdatePartial(
        Name? name,
        Description? description,
        string? contractor,
        Code? code)
    {
        if (!OrderStatus.CanEdit)
        {
            return Result<None, Exception>.Failure(
                new InvalidOperationException(
                    "Cannot modify order details when the order is not editable."));
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

        return Result<None, Exception>.Success();
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

        return Result<None, Exception>.Success();
    }
}
