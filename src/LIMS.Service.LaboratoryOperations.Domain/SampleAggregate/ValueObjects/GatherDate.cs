using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;

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
            return new GatherDate(begin, end);
        }

        if (begin.Value > end.Value)
        {
            return new ArgumentException("Gather begin date cannot be later than gather end date.");
        }

        return new GatherDate(begin, end);
    }
}
