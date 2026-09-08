using Library.Domain.SeedWork.Events;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Events;

// Не передаём всю модель
public sealed record StudyTemplatePublishedDomainEvent(StudyTemplate StudyTemplate) : IDomainEvent;
