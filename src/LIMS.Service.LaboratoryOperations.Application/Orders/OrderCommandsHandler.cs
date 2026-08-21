using Application.SeedWork;
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
    public async Task<Result<Order, Exception>> CreateAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.CastFailure<Order>();
        }

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure)
        {
            return descResult.CastFailure<Order>();
        }

        var codeResult = Code.Create(command.Code);
        if (codeResult.IsFailure)
        {
            return codeResult.CastFailure<Order>();
        }

        var createOrder = new Order(nameResult.GetValue(), descResult.GetValue(), command.Contractor,
            codeResult.GetValue());

        return await SaveNewAsync(createOrder, cancellationToken);
    }

    public async Task<Result<None, Exception>> UpdateAsync(
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
                return nameResult.CastFailure<None>();
            }

            name = nameResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descriptionResult = Description.Create(command.Description);
            if (descriptionResult.IsFailure)
            {
                return descriptionResult.CastFailure<None>();
            }

            description = descriptionResult.GetValue();
        }

        Code? code = null;
        if (command.Code is not null)
        {
            var codeResult = Code.Create(command.Code);
            if (codeResult.IsFailure)
            {
                return codeResult.CastFailure<None>();
            }

            code = codeResult.GetValue();
        }

        var updateResult = order.UpdatePartial(name, description, command.Contractor, code);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> ChangeStatusAsync(
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
            return new InvalidOperationException($"Unknown status '{command.Status}'.");
        }

        var samples = (await sampleRepository.GetByOrderIdAsync(order.Id, cancellationToken)).ToList()
            .AsReadOnly();

        var changeResult = statusChangeDomainService.ValidateAndChangeStatus(order, newStatus, samples);
        if (changeResult.IsFailure)
        {
            return changeResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> DeleteAsync(
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
            return deleteResult;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<Order, Exception>> SaveNewAsync(
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
            return new Exception($"Failed to save Order: {ex.Message}", ex);
        }
    }

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new Exception($"Failed to save Order: {ex.Message}", ex);
        }
    }

    private async Task<Result<Order, Exception>> GetOrderForChangeAsync(
        Guid id,
        CancellationToken ct)
    {
        var order = await repository.GetByIdForChangeAsync(new OrderId(id), ct);
        return order is null ? new KeyNotFoundException($"Order with id {id} not found.") : order;
    }
}
