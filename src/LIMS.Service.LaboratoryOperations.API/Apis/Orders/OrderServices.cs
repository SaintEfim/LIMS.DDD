using LIMS.Service.LaboratoryOperations.Application.Orders;

namespace LIMS.Service.LaboratoryOperations.API.Apis.Orders;

public class OrderServices(OrderCommandsHandler commands, OrderQueries queries)
{
    public OrderCommandsHandler Commands { get; } = commands;
    public OrderQueries Queries { get; } = queries;
}
