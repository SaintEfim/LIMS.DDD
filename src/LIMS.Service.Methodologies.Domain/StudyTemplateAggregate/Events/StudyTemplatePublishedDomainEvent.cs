using Library.Domain.SeedWork.Events;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Events;

public sealed record StudyTemplatePublishedDomainEvent(Guid StudyTemplateId) : IDomainEvent;
