using FraudStream.Domain.Entities;

namespace FraudStream.Application.Interfaces
{
    /// <summary>
    /// Contrato de publicação de eventos para o broker (RabbitMQ via MassTransit).
    /// Implementado na Infrastructure. A Application depende apenas deste contrato.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>Publica um único evento no broker.</summary>
        Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
            where TEvent : IDomainEvent;

        /// <summary>Publica todos os domain events coletados por uma entidade de uma vez.</summary>
        Task PublishDomainEventsAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken ct = default);
    }

}
