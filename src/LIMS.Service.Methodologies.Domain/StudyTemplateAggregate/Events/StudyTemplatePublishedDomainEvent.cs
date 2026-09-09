using Library.Domain.SeedWork.Events;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Events;

// TODO Не передаём всю модель
public sealed record StudyTemplatePublishedDomainEvent(StudyTemplate StudyTemplate) : IDomainEvent;
