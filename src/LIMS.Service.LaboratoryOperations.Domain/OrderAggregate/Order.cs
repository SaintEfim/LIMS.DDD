using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
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

    public OrderId Id { get; }

    public Name Name { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public Code Code { get; private set; } = null!;

    public string Contractor { get; private set; } = null!;

    public OrderStatus OrderStatus { get; private set; } = OrderStatus.Draft;

    public bool CanAcceptNewEntity => OrderStatus == OrderStatus.Draft || OrderStatus == OrderStatus.InProgress;

    public bool CanDeleteAssociatedEntities => OrderStatus == OrderStatus.Draft;

    public Result<None, DomainError> Delete()
    {
        if (IsDeleted)
        {
            return new EntityAlreadyDeletedError(nameof(Order), Id.Value);
        }

        if (OrderStatus != OrderStatus.Draft)
        {
            return new InvalidStatusTransitionError(nameof(Order), OrderStatus.Name, "Deleted");
        }

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;

        return new None();
    }

    public Result<None, DomainError> UpdatePartial(
        Name? name,
        Description? description,
        string? contractor,
        Code? code)
    {
        if (!OrderStatus.CanEdit)
        {
            return new EntityNotEditableError(nameof(Order), OrderStatus.Name, "modify order details");
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

    internal Result<None, InvalidStatusTransitionError> ChangeStatus(
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
