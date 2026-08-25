namespace Guides.Service.Outbox;

public sealed class OutboxMessage
{
    public required Guid Id { get; init; }

    public required string Type { get; init; }

    public required string Content { get; init; }

    public required DateTime OccurredOnUtc { get; init; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }
}
