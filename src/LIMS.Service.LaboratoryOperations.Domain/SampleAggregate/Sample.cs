using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.SoftDeletable;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;

public class Sample
    : SoftDeletableModel,
        IAggregateRoot
{
    internal Sample(
        OrderId orderId,
        Name name,
        GatherDate gatherDate,
        Code code,
        Volume volume)
    {
        Id = new SampleId(Guid.NewGuid());
        OrderId = orderId;
        Name = name;
        GatherDate = gatherDate;
        Code = code;
        Volume = volume;
        SampleStatus = SampleStatus.Registered;
    }

    public SampleId Id { get; private set; }

    public OrderId OrderId { get; private set; }

    public Name Name { get; private set; }

    public GatherDate GatherDate { get; private set; }

    public Code Code { get; private set; }

    public Volume Volume { get; private set; }

    public SampleStatus SampleStatus { get; private set; }

    public bool CanAcceptNewEntity =>
        SampleStatus == SampleStatus.Registered || SampleStatus == SampleStatus.InProgress;

    internal Result<None, Exception> Delete()
    {
        if (IsDeleted)
        {
            return new InvalidOperationException("Sample is already deleted.");
        }

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;

        return new None();
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
            return new InvalidOperationException("Cannot modify sample details when it is InWork or Completed.");
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

        return new None();
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

        return new None();
    }
}
