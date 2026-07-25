using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateDeterminations;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Queries;

public sealed record StudyTemplateDeterminationDto(Guid Id, string Unit, double? MinValue, double? MaxValue)
{
  public static StudyTemplateDeterminationDto FromDomain(
      StudyTemplateDetermination determination)
  {
      return new StudyTemplateDeterminationDto(Id: determination.Id.Value, Unit: determination.Unit, MinValue: determination.Specification.MinValue,
          MaxValue: determination.Specification.MaxValue);
  }
}
