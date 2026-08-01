using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities;

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
        return new InputParameterDto(Id: inputParameter.Id.Value, Name: inputParameter.Name.Value,
            Description: inputParameter.Description.Value, AliasName: inputParameter.AliasName.Value,
            MinValue: inputParameter.Specification.MinValue, MaxValue: inputParameter.Specification.MaxValue);
    }
}
