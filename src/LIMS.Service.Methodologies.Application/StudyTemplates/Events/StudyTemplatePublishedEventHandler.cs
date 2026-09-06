using Library.Broker.Messages;
using Library.Domain.SeedWork.Events;
using Library.Outbox;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Events;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.Events;

public class StudyTemplatePublishedEventHandler(IStudyTemplateRepository repository, IOutboxRepository outboxRepository)
    : IDomainEventHandler<StudyTemplatePublishedDomainEvent>
{
    public async Task Handle(
        StudyTemplatePublishedDomainEvent @event,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetByIdAsync(new StudyTemplateId(@event.StudyTemplateId), cancellationToken);
        if (template is null) return;

        var message = new StudyTemplatePublishedMessage(template.Id.Value, template.Name.Value,
            template.Description.Value ?? string.Empty, template.Revision.Value, template.InputParameters
                .Where(p => !p.IsDeleted)
                .Select(p => new InputParameterMessage(p.Id.Value, p.Name.Value, p.Description.Value, p.AliasName.Value,
                    p.Specification.MinValue, p.Specification.MaxValue))
                .ToList(), template.ResultDefinitions
                .Where(r => !r.IsDeleted)
                .Select(r => new ResultDefinitionMessage(r.Id.Value, r.ResultInstance, r.UnitId.Value,
                    r.Specification.MinValue, r.Specification.MaxValue))
                .ToList(), template.CalculationRules
                .Where(c => !c.IsDeleted)
                .Select(c => new CalculationRuleMessage(c.Id.Value, c.Name.Value, c.Description.Value ?? string.Empty,
                    c.FormulaExpression.Value, c.ResultDefinitionId.Value))
                .ToList());

        outboxRepository.InsertOutboxMessage(message);
    }
}
