using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.SoftDeletable;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

public sealed record StudyTemplateSnapshot(
    StudyTemplateId Id,
    Revision Revision,
    Name Name,
    Description Description,
    IReadOnlyList<InputParameterSnapshot> Parameters,
    IReadOnlyList<ResultDefinitionSnapshot> Results,
    IReadOnlyList<CalculationRuleSnapshot> CalculationRules) : SoftDeletableRecord,
    IAggregateRoot;
