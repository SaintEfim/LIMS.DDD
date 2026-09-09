using Library.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;

public sealed class MeasuredValue : ISoftDeletable
{
    internal MeasuredValue(
        StudyId studyId,
        InputParameterId inputParameterId)
    {
        Id = new MeasuredValueId(Guid.NewGuid());
        StudyId = studyId;
        InputParameterId = inputParameterId;
    }

    public MeasuredValueId Id { get; private set; }

    public StudyId StudyId { get; private set; }

    public InputParameterId InputParameterId { get; private set; }

    public double? Value { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

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
