using FraudStream.Domain.Enums;
using FraudStream.Domain.Events;
using FraudStream.Domain.Exceptions;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Domain.Entities
{
    /// <summary>
    /// Aggregate Root principal do domínio.
    /// Representa uma transação de pagamento desde o recebimento até a decisão final.
    /// Toda mutação de estado ocorre via métodos de domínio — nunca via setters públicos.
    /// </summary>
    public sealed class Transaction : BaseEntity
    {
        // ── Identificação ──────────────────────────────────────────────────────────
        public TransactionId TransactionId { get; private set; } = default!;

        // ── Value Objects ──────────────────────────────────────────────────────────
        public Money Amount { get; private set; } = default!;
        public CardInfo Card { get; private set; } = default!;
        public DeviceInfo Device { get; private set; } = default!;
        public MerchantInfo Merchant { get; private set; } = default!;

        // ── Estado ─────────────────────────────────────────────────────────────────
        public TransactionStatus Status { get; private set; } = TransactionStatus.Received;

        // ── Relacionamento com FraudDecision ───────────────────────────────────────
        public Guid? FraudDecisionId { get; private set; }
        public FraudDecision? FraudDecision { get; private set; }

        private Transaction() { } // EF Core

        /// <summary>
        /// Factory method — único ponto de criação de uma Transaction no domínio.
        /// Garante que a entidade nasce sempre em estado válido.
        /// </summary>
        public static Transaction Create(
            TransactionId transactionId,
            Money amount,
            CardInfo card,
            DeviceInfo device,
            MerchantInfo merchant)
        {
            ArgumentNullException.ThrowIfNull(transactionId);
            ArgumentNullException.ThrowIfNull(amount);
            ArgumentNullException.ThrowIfNull(card);
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(merchant);

            var transaction = new Transaction
            {
                TransactionId = transactionId,
                Amount = amount,
                Card = card,
                Device = device,
                Merchant = merchant,
                Status = TransactionStatus.Received
            };

            transaction.AddDomainEvent(new TransactionCreatedEvent(
                transactionId.Value,
                amount.Amount,
                amount.Currency,
                card.CardId,
                device.DeviceId,
                device.Country,
                merchant.MerchantId,
                transaction.CreatedAt
            ));

            return transaction;
        }

        /// <summary>Marca a transação como em análise pelo Fraud Engine.</summary>
        public void MarkAsProcessing()
        {
            if (Status != TransactionStatus.Received)
                throw new DomainException($"Transação não pode ir para Processing a partir do status {Status}.");

            Status = TransactionStatus.Processing;
            SetUpdatedAt();
        }

        /// <summary>Aplica a decisão do Fraud Engine e fecha o ciclo de vida da transação.</summary>
        public void ApplyDecision(FraudDecision decision)
        {
            ArgumentNullException.ThrowIfNull(decision);

            if (Status != TransactionStatus.Processing)
                throw new DomainException($"Decisão só pode ser aplicada enquanto a transação está em Processing. Status atual: {Status}.");

            FraudDecision = decision;
            FraudDecisionId = decision.Id;
            Status = TransactionStatus.Analyzed;
            SetUpdatedAt();

            // Publica o domain event correto dependendo da decisão
            var domainEvent = decision.Status switch
            {
                DecisionStatus.Approved or DecisionStatus.Review =>
                    (IDomainEvent)new TransactionApprovedEvent(TransactionId.Value, decision.Status, decision.Score.TotalScore),

                DecisionStatus.Blocked or DecisionStatus.FraudConfirmed =>
                    new FraudDetectedEvent(
                        TransactionId.Value,
                        decision.Status,
                        decision.Score.TotalScore,
                        decision.Score.TriggeredRules.Select(r => r.Rule.ToString()).ToList(),
                        decision.EngineVersion),

                _ => throw new DomainException($"DecisionStatus desconhecido: {decision.Status}.")
            };

            AddDomainEvent(domainEvent);
        }

        /// <summary>Marca a transação como falha — será encaminhada para DLQ.</summary>
        public void MarkAsFailed(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("O motivo da falha deve ser informado.");

            Status = TransactionStatus.Failed;
            SetUpdatedAt();

            AddDomainEvent(new TransactionFailedEvent(TransactionId.Value, reason, DateTime.UtcNow));
        }

        // ── Queries de domínio ─────────────────────────────────────────────────────
        public bool IsAnalyzed => Status == TransactionStatus.Analyzed;
        public bool IsFraud => FraudDecision?.IsFraud ?? false;
        public bool NeedsManualReview => FraudDecision?.Status == DecisionStatus.Review;
    }
}
