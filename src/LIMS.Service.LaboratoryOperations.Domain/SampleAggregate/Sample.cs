using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;
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

    // for EF Core
    private Sample()
    {
    }

    public SampleId Id { get; private set; }

    public OrderId OrderId { get; private set; }

    public Name Name { get; private set; } = null!;

    public GatherDate GatherDate { get; private set; } = null!;

    public Code Code { get; private set; } = null!;

    public Volume Volume { get; private set; } = null!;

    public SampleStatus SampleStatus { get; private set; } = null!;

    public bool CanAcceptNewEntity =>
        SampleStatus == SampleStatus.Registered || SampleStatus == SampleStatus.InProgress;

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
        Volume? volume)
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

        var newValue = volume?.Value ?? Volume.Value;
        var newUnitId = volume?.UnitId ?? Volume.UnitId;

        Volume.Update(newValue, newUnitId);

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
