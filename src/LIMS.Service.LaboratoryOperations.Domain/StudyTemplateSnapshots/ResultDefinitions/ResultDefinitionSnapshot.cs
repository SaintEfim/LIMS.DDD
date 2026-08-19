using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.SoftDeletable;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

public sealed record ResultDefinitionSnapshot(
    ResultDefinitionId Id,
    StudyTemplateId StudyTemplateId,
    string ResultInstance,
    UnitId UnitId,
    Specification Specification) : SoftDeletableRecord;
