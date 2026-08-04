using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;

public class Sample : IAggregateRoot
{
    public SampleId Id { get; private set; }

    public OrderId OrderId { get; private set; }

    public Name Name { get; private set; }

    public GatherDate GatherDate { get; private set; }

    public Code Code { get; private set; }

    public Volume Volume { get; private set; }

    public SampleStatus SampleStatus { get; private set; } = SampleStatus.Registered;

    private Sample()
    {
    }

    public static Result<Sample, Exception> Create(
        OrderId orderId,
        Name name,
        GatherDate gatherDate,
        Code code,
        Volume volume)
    {
        var sample = new Sample
        {
            Id = new SampleId(Guid.NewGuid()),
            OrderId = orderId,
            Name = name,
            GatherDate = gatherDate,
            Code = code,
            Volume = volume,
            SampleStatus = SampleStatus.Registered
        };

        return Result<Sample, Exception>.Success(sample);
    }

    public Result<Exception> UpdatePartial(
        Name? name,
        GatherDate? gatherDate,
        Code? code,
        double? volumeValue,
        string? volumeUnit)
    {
        if (!SampleStatus.CanEdit)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot modify sample details when it is InWork or Completed."));

        if (name is not null) Name = name;
        if (gatherDate is not null) GatherDate = gatherDate;
        if (code is not null) Code = code;

        var value = volumeValue ?? Volume.Value;
        var unit = volumeUnit ?? Volume.Unit;

        var volumeResult = Volume.Create(value, unit);
        if (volumeResult.IsFailure) return Result<Exception>.Failure(volumeResult.Error!);
        var volume = volumeResult.GetValue();

        Volume = volume;

        return Result<Exception>.Success();
    }

    public Result<Exception> ChangeStatus(
        SampleStatus newSampleStatus)
    {
        var result = SampleStatus.CanTransitionTo(newSampleStatus, this);

        if (result.IsFailure) return result;

        SampleStatus = newSampleStatus;

        return Result<Exception>.Success();
    }
}
