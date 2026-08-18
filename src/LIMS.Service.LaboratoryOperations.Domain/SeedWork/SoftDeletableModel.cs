namespace LIMS.Service.LaboratoryOperations.Domain.SeedWork;

public abstract class SoftDeletableModel
{
    public bool IsDeleted { get; protected set; }

    public DateTimeOffset? DeletedAt { get; protected set; }
}

public abstract record SoftDeletableRecord
{
    public bool IsDeleted { get; protected set; }

    public DateTimeOffset? DeletedAt { get; protected set; }
}
