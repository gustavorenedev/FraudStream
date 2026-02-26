using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;

namespace FraudStream.Domain.Events
{
    /// <summary>
    /// Publicado quando um analista conclui a revisão manual de uma transação.
    /// Consumido pelo AuditWorker para registrar a decisão final com rastreabilidade.
    /// </summary>
    public sealed record ManualReviewDecidedEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public string TransactionId { get; init; }
        public string AnalystId { get; init; }
        public DecisionStatus FinalDecision { get; init; }
        public string Notes { get; init; }
        public DateTime ReviewedAt { get; init; }

        public ManualReviewDecidedEvent(
            string transactionId,
            string analystId,
            DecisionStatus finalDecision,
            string notes,
            DateTime reviewedAt)
        {
            TransactionId = transactionId;
            AnalystId = analystId;
            FinalDecision = finalDecision;
            Notes = notes;
            ReviewedAt = reviewedAt;
        }
    }
}
