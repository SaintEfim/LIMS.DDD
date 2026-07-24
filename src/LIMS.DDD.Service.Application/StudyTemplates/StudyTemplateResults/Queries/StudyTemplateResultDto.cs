using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateResults;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Queries;

public sealed record StudyTemplateResultDto(Guid Id, string Unit, double? MinValue, double? MaxValue)
{
    public static StudyTemplateResultDto FromDomain(
        StudyTemplateResult result)
    {
        return new StudyTemplateResultDto(Id: result.Id.Value, Unit: result.Unit, MinValue: result.ValueRange?.MinValue,
            MaxValue: result.ValueRange?.MaxValue);
    }
}
