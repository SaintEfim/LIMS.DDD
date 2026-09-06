using Microsoft.Extensions.DependencyInjection;

namespace Library.Outbox;

public static class DependencyInjection
{
    public static void AddOutbox(
        this IServiceCollection services)
    {
        services.AddScoped<OutboxProcessor>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddHostedService<OutboxBackgroundService>();
    }
}
