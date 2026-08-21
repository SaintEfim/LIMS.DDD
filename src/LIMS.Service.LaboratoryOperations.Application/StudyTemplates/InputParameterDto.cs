using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;

namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public sealed record InputParameterDto(
    Guid Id,
    string Name,
    string? Description,
    string AliasName,
    double? SpecMin,
    double? SpecMax)
{
    public static InputParameterDto FromSnapshot(InputParameterSnapshot snapshot)
    {
        return new InputParameterDto(
            snapshot.Id.Value,
            snapshot.Name.Value,
            snapshot.Description.Value,
            snapshot.AliasName.Value,
            snapshot.Specification.MinValue,
            snapshot.Specification.MaxValue);
    }
}

