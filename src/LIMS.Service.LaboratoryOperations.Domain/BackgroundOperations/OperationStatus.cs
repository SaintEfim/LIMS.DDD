namespace LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;

public enum OperationStatus
{
    Pending,
    Running,
    Succeeded,
    Failed,
    Canceled
}
