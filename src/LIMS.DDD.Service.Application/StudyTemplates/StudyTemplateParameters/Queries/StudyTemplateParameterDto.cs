using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Parameter;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Queries;

public sealed record StudyTemplateParameterDto(
    Guid Id,
    string Name,
    string? Description,
    string? AliasName,
    double? MinValue,
    double? MaxValue)
{
    public static StudyTemplateParameterDto FromDomain(
        StudyTemplateParameter parameter)
    {
        return new StudyTemplateParameterDto(Id: parameter.Id.Value, Name: parameter.Name.Value,
            Description: parameter.Description.Value, AliasName: parameter.AliasName.Value,
            MinValue: parameter.ValueRange?.MinValue, MaxValue: parameter.ValueRange?.MaxValue);
    }
}
