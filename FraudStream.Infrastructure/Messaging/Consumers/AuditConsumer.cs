using FraudStream.Domain.Events;
using FraudStream.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FraudStream.Infrastructure.Messaging.Consumers
{
    public sealed class AuditConsumer :
    IConsumer<FraudDetectedEvent>,
    IConsumer<TransactionApprovedEvent>,
    IConsumer<ManualReviewDecidedEvent>
    {
        private readonly ITransactionRepository _repository;
        private readonly ILogger<AuditConsumer> _logger;

        public AuditConsumer(ITransactionRepository repository, ILogger<AuditConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<FraudDetectedEvent> context)
        {
            _logger.LogWarning(
                "[AUDIT] Fraude detectada. TransactionId={Id} Decision={Decision} Score={Score} Rules=[{Rules}]",
                context.Message.TransactionId,
                context.Message.Decision,
                context.Message.TotalScore,
                string.Join(", ", context.Message.TriggeredRules));

            await Task.CompletedTask;
        }

        public async Task Consume(ConsumeContext<TransactionApprovedEvent> context)
        {
            _logger.LogInformation(
                "[AUDIT] Transação aprovada. TransactionId={Id} Decision={Decision} Score={Score}",
                context.Message.TransactionId,
                context.Message.Decision,
                context.Message.TotalScore);

            await Task.CompletedTask;
        }

        public async Task Consume(ConsumeContext<ManualReviewDecidedEvent> context)
        {
            _logger.LogInformation(
                "[AUDIT] Revisão manual concluída. TransactionId={Id} Analista={Analyst} Decisão={Decision}",
                context.Message.TransactionId,
                context.Message.AnalystId,
                context.Message.FinalDecision);

            await Task.CompletedTask;
        }
    }
}
