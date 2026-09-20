using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;
using Newtonsoft.Json.Linq;

namespace LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

public sealed record CreateBackgroundOperationCommand(
    Guid RequestedByUserId,
    OperationType Type,
    JObject Payload);
