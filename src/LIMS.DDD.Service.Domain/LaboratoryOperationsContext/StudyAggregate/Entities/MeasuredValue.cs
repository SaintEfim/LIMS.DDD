using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;

public sealed class MeasuredValue
{
    private MeasuredValue()
    {
    }

    public MeasuredValueId Id { get; private set; }
    public StudyId StudyId { get; private set; }

    public InputParameterId ParameterId { get; private set; }
    public double? Value { get; private set; }

    public static MeasuredValue Create(
        StudyId studyId,
        InputParameterId parameterId,
        double? value,
        string? unit)
    {
        return new MeasuredValue
        {
            Id = new MeasuredValueId(Guid.NewGuid()),
            StudyId = studyId,
            ParameterId = parameterId,
            Value = value,
        };
    }

    public void Update(
        double? value)
    {
        if (value is not null)  Value = value;
    }
}
