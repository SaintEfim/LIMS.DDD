using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Reports.Client;

public static class DependencyInjection
{
    public static void AddReportsClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient("Reports", client =>
        {
            client.BaseAddress = new Uri(configuration["Reports:BaseUrl"] ??
                                         throw new InvalidOperationException("Reports:BaseUrl is not configured."));
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddScoped<IReportClient, ReportClient>();
    }
}
