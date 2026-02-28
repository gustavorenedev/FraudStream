using FraudStream.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FraudStream.Application.EventHandlers
{
    /// <summary>
    /// Handler in-process para FraudDetected.
    /// Registra log estruturado imediatamente quando fraude é detectada.
    /// </summary>
    public sealed class FraudDetectedEventHandler : INotificationHandler<FraudDetectedEvent>
    {
        private readonly ILogger<FraudDetectedEventHandler> _logger;

        public FraudDetectedEventHandler(ILogger<FraudDetectedEventHandler> logger)
            => _logger = logger;

        public Task Handle(FraudDetectedEvent notification, CancellationToken ct)
        {
            _logger.LogWarning(
                "[DomainEvent] FraudDetected: Id={TransactionId} Decision={Decision} Score={Score} Rules=[{Rules}]",
                notification.TransactionId,
                notification.Decision,
                notification.TotalScore,
                string.Join(", ", notification.TriggeredRules));

            return Task.CompletedTask;
        }
    }
}
