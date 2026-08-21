using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public sealed record StudyTemplateDto(
    Guid Id,
    string Revision,
    string Name,
    IReadOnlyList<InputParameterDto> Parameters,
    IReadOnlyList<ResultDefinitionDto> Results,
    IReadOnlyList<CalculationRuleDto> CalculationRules)
{
    public static StudyTemplateDto FromSnapshot(
        StudyTemplateSnapshot snapshot,
        IReadOnlyDictionary<UnitId, UnitSnapshot> unitsById)
    {
        return new StudyTemplateDto(
            snapshot.Id.Value,
            snapshot.Revision.Value,
            snapshot.Name.Value,
            snapshot.Parameters.Select(InputParameterDto.FromSnapshot).ToList(),
            snapshot.Results
                .Select(r => ResultDefinitionDto.FromSnapshot(
                    unitsById.TryGetValue(r.UnitId, out var unit) ? unit : null, r))
                .ToList(),
            snapshot.CalculationRules.Select(CalculationRuleDto.FromSnapshot).ToList());
    }
}
