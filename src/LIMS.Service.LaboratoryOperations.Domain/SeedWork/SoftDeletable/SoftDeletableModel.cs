namespace LIMS.Service.LaboratoryOperations.Domain.SeedWork.SoftDeletable;

public abstract class SoftDeletableModel
{
    public bool IsDeleted { get; protected set; }

    public DateTimeOffset? DeletedAt { get; protected set; }
}
