using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Result;

namespace LIMS.DDD.Service.API.Dtos;

public sealed record StudyTemplateResultDto(Guid Id, string Unit, double? MinValue, double? MaxValue)
{
    public static StudyTemplateResultDto FromDomain(
        StudyTemplateResult result)
    {
        return new StudyTemplateResultDto(Id: result.Id.Value, Unit: result.Unit, MinValue: result.ValueRange?.MinValue,
            MaxValue: result.ValueRange?.MaxValue);
    }
}
