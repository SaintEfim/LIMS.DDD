using Guides.Messages;
using LIMS.DDD.Service.Application.Units;
using Microsoft.Extensions.Logging;
using RabbitMq.Library.QuickStart.Receive;

namespace LIMS.DDD.Service.Infrastructure.Units.Receive;

public class UnitCreatedMessageHandler(
    UnitSnapshotCommandsHandler unitSnapshotCommandsHandler,
    ILogger<UnitCreatedMessageHandler> logger) : IReceiveHandler<UnitCreatedMessage>
{
    public async Task HandleAsync(
        UnitCreatedMessage message,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing UnitCreated event: {Name}", message.Name);

        await unitSnapshotCommandsHandler.CreateAsync(new CreateUnitSnapshotCommand(message.Id, message.Name),
            cancellationToken);
    }
}
