using Application.SeedWork;
using Application.SeedWork.Errors;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Application.Orders.Commands;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.Services;
using LIMS.Service.LaboratoryOperations.Domain.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Application.Orders;

public sealed class OrderCommandsHandler(
    IUnitOfWork unitOfWork,
    IOrderRepository repository,
    ISampleRepository sampleRepository,
    OrderStatusChangeDomainService statusChangeDomainService) : ICommandsHandler
{
    public async Task<Result<Order, ApplicationError>> CreateAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return new DomainRuleViolation(nameResult.GetError());
        }

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure)
        {
            return new DomainRuleViolation(descResult.GetError());
        }

        var codeResult = Code.Create(command.Code);
        if (codeResult.IsFailure)
        {
            return new DomainRuleViolation(codeResult.GetError());
        }

        var createOrder = new Order(nameResult.GetValue(), descResult.GetValue(), command.Contractor,
            codeResult.GetValue());

        return await SaveNewAsync(createOrder, cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> UpdateAsync(
        Guid id,
        UpdateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var orderResult = await GetOrderForChangeAsync(id, cancellationToken);
        if (orderResult.IsFailure)
        {
            return orderResult.CastFailure<None>();
        }

        var order = orderResult.GetValue();

        Name? name = null;
        if (command.Name is not null)
        {
            var nameResult = Name.Create(command.Name);
            if (nameResult.IsFailure)
            {
                return new DomainRuleViolation(nameResult.GetError());
            }

            name = nameResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descriptionResult = Description.Create(command.Description);
            if (descriptionResult.IsFailure)
            {
                return new DomainRuleViolation(descriptionResult.GetError());
            }

            description = descriptionResult.GetValue();
        }

        Code? code = null;
        if (command.Code is not null)
        {
            var codeResult = Code.Create(command.Code);
            if (codeResult.IsFailure)
            {
                return new DomainRuleViolation(codeResult.GetError());
            }

            code = codeResult.GetValue();
        }

        var updateResult = order.UpdatePartial(name, description, command.Contractor, code);
        if (updateResult.IsFailure)
        {
            return new DomainRuleViolation(updateResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> ChangeStatusAsync(
        Guid id,
        ChangeOrderStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var orderResult = await GetOrderForChangeAsync(id, cancellationToken);
        if (orderResult.IsFailure)
        {
            return orderResult.CastFailure<None>();
        }

        var order = orderResult.GetValue();

        if (!OrderStatus.TryParse(command.Status, out var newStatus) || newStatus is null)
        {
            return new ValidationError($"Unknown status '{command.Status}'.");
        }

        var samples = (await sampleRepository.GetByOrderIdAsync(order.Id, cancellationToken)).ToList()
            .AsReadOnly();

        var changeResult = statusChangeDomainService.ValidateAndChangeStatus(order, newStatus, samples);
        if (changeResult.IsFailure)
        {
            return new DomainRuleViolation(changeResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var orderResult = await GetOrderForChangeAsync(id, cancellationToken);
        if (orderResult.IsFailure)
        {
            return orderResult.CastFailure<None>();
        }

        var order = orderResult.GetValue();

        var deleteResult = order.Delete();
        if (deleteResult.IsFailure)
        {
            return new DomainRuleViolation(deleteResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<Order, ApplicationError>> SaveNewAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        try
        {
            repository.Add(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return order;
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save Order: {ex.Message}");
        }
    }

    private async Task<Result<None, ApplicationError>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save Order: {ex.Message}");
        }
    }

    private async Task<Result<Order, ApplicationError>> GetOrderForChangeAsync(
        Guid id,
        CancellationToken ct)
    {
        var order = await repository.GetByIdForChangeAsync(new OrderId(id), ct);
        if (order is null)
        {
            return new NotFoundError($"Order with id '{id}' not found.");
        }

        return order;
    }
}
