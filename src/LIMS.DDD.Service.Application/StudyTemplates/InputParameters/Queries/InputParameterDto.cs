using LIMS.DDD.Service.Domain.StudyTemplateAggregate.InputParameters;

namespace LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Queries;

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
        return new InputParameterDto(Id: inputParameter.Id.Value, Name: inputParameter.Name,
            Description: inputParameter.Description, AliasName: inputParameter.AliasName,
            MinValue: inputParameter.Specification.MinValue, MaxValue: inputParameter.Specification?.MaxValue);
    }
}
