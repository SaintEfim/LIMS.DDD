using System.Reflection;
using Library.Application.SeedWork;
using Microsoft.Extensions.DependencyInjection;

namespace Guides.Service.Application;

public static class DependencyInjection
{
    private static readonly Assembly ThisAssembly = typeof(DependencyInjection).Assembly;

    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddQueries(ThisAssembly);
        services.AddCommandsHandlers(ThisAssembly);
        services.AddEvents(ThisAssembly);
    }
}
