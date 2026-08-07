using LIMS.DDD.Service.Application.Orders;

namespace LIMS.DDD.Service.API.Apis.Orders;

public class OrderServices(OrderCommandsHandler commands, OrderQueries queries)
{
    public OrderCommandsHandler Commands { get; } = commands;
    public OrderQueries Queries { get; } = queries;
}
