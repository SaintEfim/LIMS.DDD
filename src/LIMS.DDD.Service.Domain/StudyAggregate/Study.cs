using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyAggregate.Ids;

namespace LIMS.DDD.Service.Domain.StudyAggregate;

public class Study : IAggregateRoot
{
    public StudyId Id { get; private set; }
}
