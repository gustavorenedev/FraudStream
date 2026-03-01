using FraudStream.Application.Interfaces;
using FraudStream.Domain.Entities;
using FraudStream.Domain.Events;
using FraudStream.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FraudStream.Infrastructure.Messaging.Consumers
{
    public sealed class TransactionCreatedConsumer : IConsumer<TransactionCreatedEvent>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFraudRuleEngine _fraudRuleEngine;
        private readonly IFraudDecisionRepository _fraudDecisionRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<TransactionCreatedConsumer> _logger;

        public TransactionCreatedConsumer(
            ITransactionRepository transactionRepository,
            IFraudRuleEngine fraudRuleEngine,
            IFraudDecisionRepository fraudDecisionRepository,
            IEventPublisher eventPublisher,
            ILogger<TransactionCreatedConsumer> logger)
        {
            _transactionRepository = transactionRepository;
            _fraudRuleEngine = fraudRuleEngine;
            _fraudDecisionRepository = fraudDecisionRepository;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TransactionCreatedEvent> context)
        {
            var evt = context.Message;
            var ct = context.CancellationToken;

            var transaction = await _transactionRepository.GetByTransactionIdAsync(evt.TransactionId, ct);
            if (transaction is null)
            {
                _logger.LogError("Transação {TransactionId} não encontrada no banco.", evt.TransactionId);
                return;
            }

            // 1. Marca como em processamento
            transaction.MarkAsProcessing();
            await _transactionRepository.UpdateAsync(transaction, ct);

            // 2. Executa as 7 regras em paralelo
            var scoreBreakdown = await _fraudRuleEngine.EvaluateAsync(transaction, ct);

            // 3. Cria a decisão
            var decision = FraudDecision.Create(evt.TransactionId, scoreBreakdown, engineVersion: "1.0.0");
            await _fraudDecisionRepository.AddAsync(decision, ct);

            // 4. Aplica a decisão na Transaction — dispara o domain event correto
            transaction.ApplyDecision(decision);
            await _transactionRepository.UpdateAsync(transaction, ct);

            // 5. Publica os domain events (FraudDetectedEvent ou TransactionApprovedEvent)
            await _eventPublisher.PublishDomainEventsAsync(transaction.DomainEvents, ct);
            transaction.ClearDomainEvents();
        }
    }
}
