using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.Ids;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.ValueObject;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;

public class Order
{
    public OrderId Id { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public string Contractor { get; private set; } = string.Empty;

    public OrderStatus OrderStatus { get; private set; } = OrderStatus.Draft;

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
            Name = name,
            Description = description,
            Contractor = contractor
        };

        return Result<Order, Exception>.Success(order);
    }

    public Result<Order, Exception> UpdatePartial(
        Name? name,
        Description? description,
        string? contractor)
    {
        if (!OrderStatus.CanEdit)
            return Result<Order, Exception>.Failure(
                new InvalidOperationException(
                    "Cannot modify details of an Active or Archived template. Create a new revision."));

        if (name is not null) Name = name.Value;
        if (description is not null) Description = description.Value;
        if (contractor is not null) Contractor = contractor;

        return Result<Order, Exception>.Success(this);
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
