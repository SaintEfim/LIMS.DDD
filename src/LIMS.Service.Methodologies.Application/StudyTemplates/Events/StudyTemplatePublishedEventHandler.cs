using Library.Broker.Messages;
using Library.Domain.SeedWork.Events;
using Library.Outbox;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Events;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.Events;

public class StudyTemplatePublishedDomainEventHandler(IOutboxRepository outboxRepository)
    : IDomainEventHandler<StudyTemplatePublishedDomainEvent>
{
    public Task Handle(
        StudyTemplatePublishedDomainEvent @event,
        CancellationToken cancellationToken = default)
    {
        var template = @event.StudyTemplate;

        var message = new StudyTemplatePublishedMessage(template.Id.Value, template.Name.Value,
            template.Description.Value, template.Revision.Value, template.InputParameters
                .Where(p => !p.IsDeleted)
                .Select(p => new InputParameterMessage(p.Id.Value, p.Name.Value, p.Description.Value, p.AliasName.Value,
                    p.Specification.MinValue, p.Specification.MaxValue))
                .ToList(), template.ResultDefinitions
                .Where(r => !r.IsDeleted)
                .Select(r => new ResultDefinitionMessage(r.Id.Value, r.ResultInstance, r.UnitId.Value,
                    r.Specification.MinValue, r.Specification.MaxValue))
                .ToList(), template.CalculationRules
                .Where(c => !c.IsDeleted)
                .Select(c => new CalculationRuleMessage(c.Id.Value, c.Name.Value, c.Description.Value,
                    c.FormulaExpression.Value, c.ResultDefinitionId.Value))
                .ToList());

        outboxRepository.InsertOutboxMessage(message);

        return Task.CompletedTask;
    }
}
