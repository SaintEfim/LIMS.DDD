using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application.SeedWork;

public static class DependencyInjection
{
    extension(
        IServiceCollection services)
    {
        public void AddCommandsHandlers(
            Assembly assembly)
        {
            var interfaceType = assembly.DefinedTypes.Where(x =>
                x.IsClass && x.ImplementedInterfaces.Contains(typeof(ICommandsHandler)));

            foreach (var type in interfaceType) services.AddScoped(type.AsType());
        }

        public void AddQueries(
            Assembly assembly)
        {
            var interfaceType =
                assembly.DefinedTypes.Where(x => x.IsClass && x.ImplementedInterfaces.Contains(typeof(IQueries)));

            foreach (var type in interfaceType) services.AddScoped(type.AsType());
        }
    }
}
