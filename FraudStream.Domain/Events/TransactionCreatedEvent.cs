using FraudStream.Domain.Entities;

namespace FraudStream.Domain.Events
{
    /// <summary>
    /// Publicado quando uma transação é recebida pelo FraudStream.Api.
    /// Consumido pelo FraudDetectionWorker e pelo AuditWorker em paralelo.
    /// </summary>
    public sealed record TransactionCreatedEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public string TransactionId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; }
        public string CardId { get; init; }
        public string DeviceId { get; init; }
        public string Country { get; init; }
        public string MerchantId { get; init; }
        public DateTime TransactionOccurredAt { get; init; }

        public TransactionCreatedEvent(
            string transactionId,
            decimal amount,
            string currency,
            string cardId,
            string deviceId,
            string country,
            string merchantId,
            DateTime transactionOccurredAt)
        {
            TransactionId = transactionId;
            Amount = amount;
            Currency = currency;
            CardId = cardId;
            DeviceId = deviceId;
            Country = country;
            MerchantId = merchantId;
            TransactionOccurredAt = transactionOccurredAt;
        }
    }
}
