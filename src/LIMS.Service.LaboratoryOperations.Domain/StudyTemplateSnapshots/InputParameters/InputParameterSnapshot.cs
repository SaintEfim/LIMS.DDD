using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;

public sealed record InputParameterSnapshot(
    InputParameterId Id,
    StudyTemplateId StudyTemplateId,
    Name Name,
    Description Description,
    AliasName AliasName,
    Specification Specification) : SoftDeletableRecord;
