namespace LIMS.Service.LaboratoryOperations.Application.Orders.Commands;

public sealed record UpdateOrderCommand(string? Name, string? Description, string? Contractor, string? Code);
