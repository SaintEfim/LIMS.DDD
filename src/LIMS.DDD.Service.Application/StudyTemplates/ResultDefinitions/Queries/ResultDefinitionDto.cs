using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ResultDefinitions;

namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Queries;

public sealed record ResultDefinitionDto(Guid Id, string Unit, double? MinValue, double? MaxValue)
{
  public static ResultDefinitionDto FromDomain(
      ResultDefinition resultDefinition)
  {
      return new ResultDefinitionDto(Id: resultDefinition.Id.Value, Unit: resultDefinition.Unit, MinValue: resultDefinition.Specification.MinValue,
          MaxValue: resultDefinition.Specification.MaxValue);
  }
}
