using MediatR;

namespace FraudStream.Domain.Entities
{
    // Interface marcadora que todo domain event implementa. Define apenas EventId e OccurredAt. Serve para que a BaseEntity possa guardar uma lista genérica de eventos sem conhecer os tipos concretos.
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
    }
}
