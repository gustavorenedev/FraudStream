using FraudStream.Application.Interfaces;
using FraudStream.Domain.Entities;
using MassTransit;

namespace FraudStream.Infrastructure.Messaging.Publishers
{
    public sealed class EventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public EventPublisher(IPublishEndpoint publishEndpoint)
            => _publishEndpoint = publishEndpoint;

        public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
            where TEvent : IDomainEvent
            => await _publishEndpoint.Publish(domainEvent, ct);

        public async Task PublishDomainEventsAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken ct = default)
        {
            foreach (var domainEvent in domainEvents)
                await _publishEndpoint.Publish(domainEvent, domainEvent.GetType(), ct);
        }
    }
}
