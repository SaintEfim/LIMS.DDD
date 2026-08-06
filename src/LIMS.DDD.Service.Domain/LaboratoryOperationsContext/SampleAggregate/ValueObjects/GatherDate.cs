using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;

public sealed record GatherDate
{
    private GatherDate(
        DateTimeOffset? begin,
        DateTimeOffset? end)
    {
        Begin = begin;
        End = end;
    }

    public DateTimeOffset? Begin { get; init; }
    public DateTimeOffset? End { get; init; }

    public static Result<GatherDate, Exception> Create(
        DateTimeOffset? begin,
        DateTimeOffset? end)
    {
        if (!begin.HasValue || !end.HasValue)
        {
            return Result<GatherDate, Exception>.Success(new GatherDate(begin, end));
        }

        if (begin.Value > end.Value)
        {
            return Result<GatherDate, Exception>.Failure(
                new ArgumentException("Gather begin date cannot be later than gather end date."));
        }

        return Result<GatherDate, Exception>.Success(new GatherDate(begin, end));
    }
}
