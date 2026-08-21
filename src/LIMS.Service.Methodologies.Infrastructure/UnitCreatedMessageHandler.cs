using RannitMq.Messages;
using LIMS.Service.Methodologies.Application.Units;
using Microsoft.Extensions.Logging;
using RabbitMq.Library.QuickStart.Receive;

namespace LIMS.Service.Methodologies.Infrastructure;

public class UnitCreatedMessageHandler(
    UnitSnapshotCommandsHandler unitSnapshotCommandsHandler,
    ILogger<UnitCreatedMessageHandler> logger) : IReceiveHandler<UnitCreatedMessage>
{
    public async Task HandleAsync(
        UnitCreatedMessage message,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing UnitCreated event: {Name}", message.Name);

        var unitResult =
            await unitSnapshotCommandsHandler.CreateAsync(new CreateUnitSnapshotCommand(message.Id, message.Name),
                cancellationToken);

        if (unitResult.IsFailure)
        {
            logger.LogError("Failed to create UnitCreated event: {Name}, Id {Id}", message.Name, message.Id);
        }
    }
}
