using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;

namespace FraudStream.Domain.Interfaces
{
    /// <summary>
    /// Contrato do repositório de FraudDecision.
    /// </summary>
    public interface IFraudDecisionRepository
    {
        Task<FraudDecision?> GetByTransactionIdAsync(string transactionId, CancellationToken ct = default);
        Task<IEnumerable<FraudDecision>> GetByStatusAsync(DecisionStatus status, int limit = 100, CancellationToken ct = default);
        Task<FraudSummary> GetSummaryAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task AddAsync(FraudDecision decision, CancellationToken ct = default);
        Task UpdateAsync(FraudDecision decision, CancellationToken ct = default);
    }

    /// <summary>Projeção de leitura para o dashboard — não é uma entidade, é um DTO de domínio.</summary>
    public sealed record FraudSummary(
        int Total,
        int Approved,
        int Review,
        int Blocked,
        int FraudConfirmed,
        decimal FraudRatePercent,
        DateTime From,
        DateTime To
    );
}
