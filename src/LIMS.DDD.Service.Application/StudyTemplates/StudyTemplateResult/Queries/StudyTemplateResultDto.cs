namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResult.Queries;

public sealed record StudyTemplateResultDto(Guid Id, string Unit, double? MinValue, double? MaxValue)
{
    public static StudyTemplateResultDto FromDomain(
        Domain.StudyTemplateAggregate.Result.StudyTemplateResult result)
    {
        return new StudyTemplateResultDto(Id: result.Id.Value, Unit: result.Unit, MinValue: result.ValueRange?.MinValue,
            MaxValue: result.ValueRange?.MaxValue);
    }
}
