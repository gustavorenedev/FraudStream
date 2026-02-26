using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;

namespace FraudStream.Domain.Events
{
    /// <summary>
    /// Publicado pelo FraudEngine quando score >= 61.
    /// Consumido pelo NotificationWorker e pelo AuditWorker para atualização de status.
    /// </summary>
    public sealed record FraudDetectedEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public string TransactionId { get; init; }
        public DecisionStatus Decision { get; init; }
        public int TotalScore { get; init; }
        public IReadOnlyList<string> TriggeredRules { get; init; }
        public string EngineVersion { get; init; }

        public FraudDetectedEvent(
            string transactionId,
            DecisionStatus decision,
            int totalScore,
            IReadOnlyList<string> triggeredRules,
            string engineVersion)
        {
            TransactionId = transactionId;
            Decision = decision;
            TotalScore = totalScore;
            TriggeredRules = triggeredRules;
            EngineVersion = engineVersion;
        }
    }
}
