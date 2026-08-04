namespace LIMS.DDD.Service.Application.Orders.Commands;

public sealed record CreateOrderCommand(string Name, string Description, string Contractor);
