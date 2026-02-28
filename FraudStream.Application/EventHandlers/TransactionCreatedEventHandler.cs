using FraudStream.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FraudStream.Application.EventHandlers
{
    /// <summary>
    /// Handler in-process de domain event — executa na mesma thread do Command.
    /// Serve para side effects síncronos internos, como logs imediatos.
    /// A publicação no RabbitMQ é feita separadamente pelo IEventPublisher.
    /// </summary>
    public sealed class TransactionCreatedEventHandler : INotificationHandler<TransactionCreatedEvent>
    {
        private readonly ILogger<TransactionCreatedEventHandler> _logger;

        public TransactionCreatedEventHandler(ILogger<TransactionCreatedEventHandler> logger)
            => _logger = logger;

        public Task Handle(TransactionCreatedEvent notification, CancellationToken ct)
        {
            _logger.LogDebug(
                "[DomainEvent] TransactionCreated: Id={TransactionId} Amount={Amount} {Currency} Card={CardId}",
                notification.TransactionId,
                notification.Amount,
                notification.Currency,
                notification.CardId);

            return Task.CompletedTask;
        }
    }
}
