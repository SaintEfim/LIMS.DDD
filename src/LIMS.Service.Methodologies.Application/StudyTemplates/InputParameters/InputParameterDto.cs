using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.InputParameters;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.InputParameters;

public sealed record InputParameterDto(
    Guid Id,
    string Name,
    string? Description,
    string? AliasName,
    double? MinValue,
    double? MaxValue)
{
    public static InputParameterDto FromDomain(
        InputParameter inputParameter)
    {
        return new InputParameterDto(inputParameter.Id.Value, inputParameter.Name.Value,
            inputParameter.Description.Value, inputParameter.AliasName.Value, inputParameter.Specification.MinValue,
            inputParameter.Specification.MaxValue);
    }
}
