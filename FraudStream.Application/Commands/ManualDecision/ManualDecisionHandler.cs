using FraudStream.Application.Common;
using FraudStream.Application.Exceptions;
using FraudStream.Application.Interfaces;
using FraudStream.Domain.Entities;
using FraudStream.Domain.Events;
using FraudStream.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FraudStream.Application.Commands.ManualDecision
{
    /// <summary>
    /// Aplica a decisão manual de um analista sobre uma transação em revisão.
    ///
    /// Fluxo:
    ///   1. Busca a Transaction pelo TransactionId
    ///   2. Chama FraudDecision.ApplyManualReview() — Domain valida as regras
    ///   3. Persiste a decisão atualizada
    ///   4. Publica ManualReviewDecidedEvent para o AuditWorker
    /// </summary>
    public sealed class ManualDecisionHandler : IRequestHandler<ManualDecisionCommand, Result>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFraudDecisionRepository _fraudDecisionRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<ManualDecisionHandler> _logger;

        public ManualDecisionHandler(
            ITransactionRepository transactionRepository,
            IFraudDecisionRepository fraudDecisionRepository,
            IEventPublisher eventPublisher,
            ILogger<ManualDecisionHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _fraudDecisionRepository = fraudDecisionRepository;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<Result> Handle(ManualDecisionCommand command, CancellationToken ct)
        {
            var transaction = await _transactionRepository
                .GetByTransactionIdAsync(command.TransactionId, ct);

            if (transaction is null)
                throw new NotFoundException(nameof(Transaction), command.TransactionId);

            if (transaction.FraudDecision is null)
                return Result.Failure("Transação ainda não possui decisão do engine. Aguarde o processamento.");

            try
            {
                // Domain valida: só aceita revisão em status Review
                transaction.FraudDecision.ApplyManualReview(
                    command.AnalystId,
                    command.FinalDecision,
                    command.Notes);

                await _fraudDecisionRepository.UpdateAsync(transaction.FraudDecision, ct);

                var reviewEvent = new ManualReviewDecidedEvent(
                    command.TransactionId,
                    command.AnalystId,
                    command.FinalDecision,
                    command.Notes,
                    DateTime.UtcNow);

                await _eventPublisher.PublishAsync(reviewEvent, ct);

                _logger.LogInformation(
                    "Revisão manual aplicada. TransactionId={TransactionId} Analista={AnalystId} Decisão={Decision}",
                    command.TransactionId, command.AnalystId, command.FinalDecision);

                return Result.Success();
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
