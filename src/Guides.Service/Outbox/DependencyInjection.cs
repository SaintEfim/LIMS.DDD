using Microsoft.EntityFrameworkCore;

namespace Guides.Service.Outbox;

public static class OutboxDependencyInjection
{
    public static void AddOutbox<TDbContext>(
        this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddSingleton<OutboxSignal>();

        services.AddScoped<OutboxSaveChangesInterceptor>();
        services.AddScoped<OutboxProcessor<TDbContext>>();

        services.AddHostedService<OutboxBackgroundService<TDbContext>>();

        services.AddDbContext<TDbContext>((
            serviceProvider,
            options) =>
        {
            options.AddInterceptors(serviceProvider.GetRequiredService<OutboxSaveChangesInterceptor>());
        });
    }
}
