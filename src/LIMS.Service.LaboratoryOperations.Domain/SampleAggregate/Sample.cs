using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;

public class Sample
    : SoftDeletableModel,
        IAggregateRoot
{
    private Sample()
    {
    }

    public SampleId Id { get; private set; }

    public OrderId OrderId { get; private set; }

    public Name Name { get; private set; } = null!;

    public GatherDate GatherDate { get; private set; } = null!;

    public Code Code { get; private set; } = null!;

    public Volume Volume { get; private set; } = null!;

    public SampleStatus SampleStatus { get; private set; } = SampleStatus.Registered;

    public bool CanAcceptNewEntity =>
        SampleStatus == SampleStatus.Registered || SampleStatus == SampleStatus.InProgress;

    internal static Result<Sample, Exception> Create(
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

    internal Result<None, Exception> Delete()
    {
        if (IsDeleted)
        {
            return Result<None, Exception>.Failure(new InvalidOperationException("Sample is already deleted."));
        }

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;

        return Result<None, Exception>.Success();
    }

    public Result<None, Exception> UpdatePartial(
        Name? name,
        GatherDate? gatherDate,
        Code? code,
        double? volumeValue,
        string? volumeUnit)
    {
        if (!SampleStatus.CanEdit)
        {
            return Result<None, Exception>.Failure(
                new InvalidOperationException("Cannot modify sample details when it is InWork or Completed."));
        }

        if (name is not null)
        {
            Name = name;
        }

        if (gatherDate is not null)
        {
            GatherDate = gatherDate;
        }

        if (code is not null)
        {
            Code = code;
        }

        var value = volumeValue ?? Volume.Value;
        var unit = volumeUnit ?? Volume.Unit;

        var volumeResult = Volume.Create(value, unit);
        if (volumeResult.IsFailure)
        {
            return volumeResult.CastFailure<None>();
        }

        var volume = volumeResult.GetValue();

        Volume = volume;

        return Result<None, Exception>.Success();
    }

    internal Result<None, Exception> ChangeStatus(
        SampleStatus newSampleStatus)
    {
        var result = SampleStatus.CanTransitionTo(newSampleStatus, this);

        if (result.IsFailure)
        {
            return result.CastFailure<None>();
        }

        SampleStatus = newSampleStatus;

        return Result<None, Exception>.Success();
    }
}
