using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;

public sealed class MeasuredValue : SoftDeletableModel

{
    private MeasuredValue()
    {
    }

    public MeasuredValueId Id { get; private set; }

    public StudyId StudyId { get; private set; }

    public ParameterSnapshot ParameterSnapshot { get; private set; } = null!;

    public double? Value { get; private set; }

    internal static MeasuredValue Create(
        StudyId studyId,
        ParameterSnapshot snapshot,
        double? value)
    {
        return new MeasuredValue
        {
            Id = new MeasuredValueId(Guid.NewGuid()),
            StudyId = studyId,
            ParameterSnapshot = snapshot,
            Value = value,
        };
    }

    internal void Update(
        double? value)
    {
        if (value is not null) Value = value;
    }

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
