namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public record CreateStudyTemplateSnapshotCommand(
    Guid Id,
    string Name,
    string Description,
    string Revision,
    IReadOnlyList<InputParameterDto> InputParameters,
    IReadOnlyList<CreateResultDefinitionCommand> ResultDefinitions,
    IReadOnlyList<CalculationRuleDto> CalculationRules);
