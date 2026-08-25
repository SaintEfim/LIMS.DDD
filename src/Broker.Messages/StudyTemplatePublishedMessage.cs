using RabbitMq.Library.Broker;

namespace Broker.Messages;

[IntegrationEvent("study-template.published")]
public record StudyTemplatePublishedMessage(
    Guid Id,
    string Name,
    string Description,
    string Revision,
    IReadOnlyList<InputParameterMessage> InputParameters,
    IReadOnlyList<ResultDefinitionMessage> ResultDefinitions,
    IReadOnlyList<CalculationRuleMessage> CalculationRules) : IIntegrationEvent;

public record InputParameterMessage(
    Guid Id,
    string Name,
    string? Description,
    string AliasName,
    double? MinValue,
    double? MaxValue);

public record ResultDefinitionMessage(Guid Id, string ResultInstance, Guid UnitId, double? MinValue, double? MaxValue);

public record CalculationRuleMessage(
    Guid Id,
    string Name,
    string? Description,
    string FormulaExpression,
    Guid ResultDefinitionId);
