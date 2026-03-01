using FraudStream.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FraudStream.Infrastructure.Messaging.Consumers
{
    public sealed class NotificationConsumer : IConsumer<FraudDetectedEvent>
    {
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(ILogger<NotificationConsumer> logger)
            => _logger = logger;

        public Task Consume(ConsumeContext<FraudDetectedEvent> context)
        {
            var evt = context.Message;

            _logger.LogWarning(
                "[NOTIFICATION] 🚨 Alerta de fraude: TransactionId={Id} Decisão={Decision} Score={Score} Regras=[{Rules}]",
                evt.TransactionId,
                evt.Decision,
                evt.TotalScore,
                string.Join(", ", evt.TriggeredRules));

            // v2: chamar IEmailService, IWebhookService, IPushNotificationService
            return Task.CompletedTask;
        }
    }
}
