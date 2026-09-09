using System.Reflection;
using Library.Application.SeedWork.Events;
using Library.Domain.SeedWork.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.SeedWork;

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

            foreach (var type in interfaceType)
            {
                services.AddScoped(type.AsType());
            }
        }

        public void AddQueries(
            Assembly assembly)
        {
            var interfaceType =
                assembly.DefinedTypes.Where(x => x.IsClass && x.ImplementedInterfaces.Contains(typeof(IQueries)));

            foreach (var type in interfaceType)
            {
                services.AddScoped(type.AsType());
            }
        }

        public void AddEvents(
            Assembly assembly)
        {
            var handlerTypes = assembly.DefinedTypes
                .Where(x => x is { IsClass: true, IsAbstract: false } &&
                            x.ImplementedInterfaces.Any(i =>
                                i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)));

            foreach (var type in handlerTypes)
            {
                var interfaceType = type.ImplementedInterfaces
                    .First(i => i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

                services.AddScoped(interfaceType, type.AsType());
            }

            services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        }
    }
}
