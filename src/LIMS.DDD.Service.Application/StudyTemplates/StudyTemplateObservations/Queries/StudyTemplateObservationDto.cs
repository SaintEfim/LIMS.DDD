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
        return new StudyTemplateObservationDto(Id: observation.Id.Value, Name: observation.Name,
            Description: observation.Description, AliasName: observation.AliasName,
            MinValue: observation.Specification?.MinValue, MaxValue: observation.Specification?.MaxValue);
    }
}
