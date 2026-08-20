using Domain.SeedWork.SeedWork.SoftDeletable;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

public sealed record ResultDefinitionSnapshot(
    ResultDefinitionId Id,
    StudyTemplateId StudyTemplateId,
    string ResultInstance,
    UnitId UnitId,
    Specification Specification) : SoftDeletableRecord;
