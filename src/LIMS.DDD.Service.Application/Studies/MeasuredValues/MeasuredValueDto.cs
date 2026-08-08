using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;

namespace LIMS.DDD.Service.Application.Studies.MeasuredValues;

public sealed record MeasuredValueDto(
    Guid Id,
    Guid StudyId,
    Guid ParameterId,
    string ParameterName,
    string ParameterAlias,
    double? SpecMin,
    double? SpecMax,
    double? Value)
{
    public static MeasuredValueDto FromDomain(
        MeasuredValue mv)
    {
        return new MeasuredValueDto(mv.Id.Value, mv.StudyId.Value, mv.ParameterSnapshot.InputParameterId,
            mv.ParameterSnapshot.Name.Value, mv.ParameterSnapshot.AliasName.Value,
            mv.ParameterSnapshot.Specification.MinValue, mv.ParameterSnapshot.Specification.MaxValue, mv.Value);
    }
}
