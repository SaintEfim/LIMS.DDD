using Guides.Messages;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public interface IIntegrationEventRegistry
{
    IntegrationEventDescriptor Get<T>()
        where T : IIntegrationEvent;
}
