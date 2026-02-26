using FraudStream.Domain.Entities;

namespace FraudStream.Domain.Events
{
    /// <summary>
    /// Publicado quando o processamento de uma transação falha após todas as tentativas.
    /// A mensagem original é encaminhada para a Dead Letter Queue.
    /// </summary>
    public sealed record TransactionFailedEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public string TransactionId { get; init; }
        public string Reason { get; init; }
        public DateTime FailedAt { get; init; }

        public TransactionFailedEvent(string transactionId, string reason, DateTime failedAt)
        {
            TransactionId = transactionId;
            Reason = reason;
            FailedAt = failedAt;
        }
    }
}
