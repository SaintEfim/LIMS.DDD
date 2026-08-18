namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public record CreateStudyTemplateSnapshotCommand(
    Guid Id,
    string Name,
    string Description,
    string Revision,
    IReadOnlyList<InputParameterDto> InputParameters,
    IReadOnlyList<ResultDefinitionDto> ResultDefinitions,
    IReadOnlyList<CalculationRuleDto> CalculationRules);
