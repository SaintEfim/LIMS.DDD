using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Reports.Client;

public static class DependencyInjection
{
    public static void AddReportsClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var keycloakAuthority = configuration["Reports:ServiceClient:Authority"] ??
                                throw new InvalidOperationException("Reports:ServiceClient:Authority is not configured.");

        services.AddHttpClient("ReportsKeycloak", client =>
        {
            client.BaseAddress = new Uri($"{keycloakAuthority.TrimEnd('/')}/");
        });
        services.AddSingleton<ServiceAccessTokenProvider>();
        services.AddTransient<UserAccessTokenHandler>();

        services.AddHttpClient("Reports", client =>
        {
            client.BaseAddress = new Uri(configuration["Reports:BaseUrl"] ??
                                         throw new InvalidOperationException("Reports:BaseUrl is not configured."));
            client.Timeout = TimeSpan.FromSeconds(60);
        })
        .AddHttpMessageHandler<UserAccessTokenHandler>();

        services.AddScoped<IReportClient, ReportClient>();
    }
}
