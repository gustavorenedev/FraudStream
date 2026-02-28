using FraudStream.Domain.Enums;

namespace FraudStream.Application.DTOs
{
    /// <summary>
    /// DTO de leitura de uma Transaction com sua decisão.
    /// Retornado pelos endpoints GET /api/transactions/{id}.
    /// </summary>
    public sealed record TransactionDto(
        string TransactionId,
        decimal Amount,
        string Currency,
        string CardId,
        string CardLastFour,
        string DeviceId,
        string Country,
        string MerchantId,
        string MerchantName,
        TransactionStatus Status,
        DateTime CreatedAt,
        FraudDecisionDto? Decision
    );
}
