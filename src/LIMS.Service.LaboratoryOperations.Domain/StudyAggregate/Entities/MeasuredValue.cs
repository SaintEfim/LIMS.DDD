using Domain.SeedWork.SeedWork.SoftDeletable;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;

public sealed class MeasuredValue : SoftDeletableModel
{
    private MeasuredValue()
    {
    }

    public MeasuredValueId Id { get; private set; }

    public StudyId StudyId { get; private set; }

    public InputParameterId InputParameterId { get; private set; }

    public double? Value { get; private set; }

    internal static MeasuredValue Create(
        StudyId studyId,
        InputParameterId inputParameterId)
    {
        return new MeasuredValue
        {
            Id = new MeasuredValueId(Guid.NewGuid()),
            StudyId = studyId,
            InputParameterId = inputParameterId
        };
    }

    internal void Update(
        double? value)
    {
        if (value is not null)
        {
            Value = value;
        }
    }

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
