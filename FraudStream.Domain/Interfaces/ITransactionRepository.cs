using System.Transactions;

namespace FraudStream.Domain.Interfaces
{
    /// <summary>
    /// Contrato do repositório de Transaction.
    /// Implementado na camada de Infrastructure — o domínio não conhece EF Core nem SQL.
    /// </summary>
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByTransactionIdAsync(string transactionId, CancellationToken ct = default);
        Task<IEnumerable<Transaction>> GetByStatusAsync(TransactionStatus status, CancellationToken ct = default);
        Task<IEnumerable<Transaction>> GetByCardIdAsync(string cardId, int limit = 50, CancellationToken ct = default);
        Task AddAsync(Transaction transaction, CancellationToken ct = default);
        Task UpdateAsync(Transaction transaction, CancellationToken ct = default);
    }
}
