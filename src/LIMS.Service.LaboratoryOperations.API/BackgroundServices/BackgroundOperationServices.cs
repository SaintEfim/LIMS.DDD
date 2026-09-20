using LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

namespace LIMS.Service.LaboratoryOperations.API.BackgroundServices;

public class BackgroundOperationServices(BackgroundOperationCommandsHandler commands, BackgroundOperationQueries queries)
{
    public BackgroundOperationCommandsHandler Commands { get; } = commands;
    public BackgroundOperationQueries Queries { get; } = queries;
}
