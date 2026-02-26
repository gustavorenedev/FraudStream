using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;

namespace FraudStream.Domain.Events
{
    /// <summary>
    /// Publicado pelo FraudEngine quando score <= 60 (Approved ou Review).
    /// Consumido pelo AuditWorker para atualização de status.
    /// </summary>
    public sealed record TransactionApprovedEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public string TransactionId { get; init; }
        public DecisionStatus Decision { get; init; }
        public int TotalScore { get; init; }

        public TransactionApprovedEvent(string transactionId, DecisionStatus decision, int totalScore)
        {
            TransactionId = transactionId;
            Decision = decision;
            TotalScore = totalScore;
        }
    }
}
