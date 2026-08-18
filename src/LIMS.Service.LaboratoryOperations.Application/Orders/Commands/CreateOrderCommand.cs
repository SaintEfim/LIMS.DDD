namespace LIMS.Service.LaboratoryOperations.Application.Orders.Commands;

public sealed record CreateOrderCommand(string Name, string Description, string Contractor, string Code);
