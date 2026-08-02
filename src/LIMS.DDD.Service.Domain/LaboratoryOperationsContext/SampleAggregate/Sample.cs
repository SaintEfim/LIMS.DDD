using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.Ids;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;

public class Sample
{
    public SampleId Id { get; private set; }

    public Name Name { get; set; }

    public GatherDate GatherDate { get; set; }

    public string? Code { get; set; }

    public double? Value { get; set; }

    public string? Unit { get; set; }
}
