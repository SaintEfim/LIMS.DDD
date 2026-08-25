using Broker.Messages;
using LIMS.Service.LaboratoryOperations.Application.StudyTemplates;
using Microsoft.Extensions.Logging;
using RabbitMq.Library.Broker;
using RabbitMq.Library.Broker.Receive;

namespace LIMS.Service.LaboratoryOperations.Infrastructure;

public class StudyTemplatePublishedMessageHandler(
    StudyTemplateSnapshotCommandsHandler snapshotCommandsHandler,
    ILogger<StudyTemplatePublishedMessageHandler> logger) : IReceiveHandler<StudyTemplatePublishedMessage>
{
    public async Task HandleAsync(
        StudyTemplatePublishedMessage message,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing StudyTemplatePublished event: {TemplateId}, Revision: {Revision}", message.Id,
            message.Revision);

        var command = new CreateStudyTemplateSnapshotCommand(message.Id, message.Name, message.Description,
            message.Revision, message.InputParameters
                .Select(p => new InputParameterDto(p.Id, p.Name, p.Description, p.AliasName, p.MinValue, p.MaxValue))
                .ToList(), message.ResultDefinitions
                .Select(r =>
                    new CreateResultDefinitionCommand(r.Id, r.ResultInstance, r.UnitId, r.MinValue, r.MaxValue))
                .ToList(), message.CalculationRules
                .Select(c =>
                    new CalculationRuleDto(c.Id, c.Name, c.Description, c.FormulaExpression, c.ResultDefinitionId))
                .ToList());

        var result = await snapshotCommandsHandler.CreateAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("Failed to create StudyTemplateSnapshot: {TemplateId}. Error: {Error}", message.Id, result
                .GetError()
                .Message);
        }
        else
        {
            logger.LogInformation("StudyTemplateSnapshot created successfully: {TemplateId}", message.Id);
        }
    }
}
