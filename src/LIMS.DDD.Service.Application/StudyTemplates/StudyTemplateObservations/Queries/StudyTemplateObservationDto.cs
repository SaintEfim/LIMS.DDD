using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateObservations;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Queries;

public sealed record StudyTemplateObservationDto(
    Guid Id,
    string Name,
    string? Description,
    string? AliasName,
    double? MinValue,
    double? MaxValue)
{
    public static StudyTemplateObservationDto FromDomain(
        StudyTemplateObservation observation)
    {
        return new StudyTemplateObservationDto(Id: observation.Id.Value, Name: observation.Name.Value,
            Description: observation.Description.Value, AliasName: observation.AliasName.Value,
            MinValue: observation.Specification?.MinValue, MaxValue: observation.Specification?.MaxValue);
    }
}
