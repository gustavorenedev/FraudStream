using Dapper;
using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FraudStream.Infrastructure.Persistence.Repositories
{
    public sealed class TransactionRepository : ITransactionRepository
    {
        private readonly FraudStreamDbContext _context;
        private readonly IDbConnection _db;

        public TransactionRepository(FraudStreamDbContext context, IDbConnection db)
        {
            _context = context;
            _db = db;
        }

        public async Task<Transaction?> GetByTransactionIdAsync(string transactionId, CancellationToken ct = default)
            => await _context.Transactions
                .Include(t => t.FraudDecision)
                .FirstOrDefaultAsync(t => t.TransactionId.Value == transactionId, ct);

        public async Task<IEnumerable<Transaction>> GetByStatusAsync(TransactionStatus status, CancellationToken ct = default)
            => await _context.Transactions
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);

        public async Task<IEnumerable<Transaction>> GetByCardIdAsync(string cardId, int limit = 50, CancellationToken ct = default)
            => await _context.Transactions
                .Where(t => t.Card.CardId == cardId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToListAsync(ct);

        public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
        {
            await _context.Transactions.AddAsync(transaction, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync(ct);
        }

        // Read side — Dapper com COUNT(*) OVER() para paginação eficiente em uma única query
        public async Task<PagedRepositoryResult<Transaction>> GetPagedAsync(
            int page,
            int pageSize,
            DecisionStatus? status = null,
            string? cardId = null,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken ct = default)
        {
            var sql = """
            SELECT
                t.*,
                COUNT(*) OVER() AS total_count
            FROM transactions t
            LEFT JOIN fraud_decisions fd ON fd.transaction_id = t.transaction_id
            WHERE
                (@Status   IS NULL OR fd.status   = @Status)
                AND (@CardId IS NULL OR t.card_id  = @CardId)
                AND t.created_at BETWEEN @From AND @To
            ORDER BY t.created_at DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

            var rows = await _db.QueryAsync(sql, new
            {
                Status = status?.ToString(),
                CardId = cardId,
                From = from ?? DateTime.UtcNow.AddHours(-24),
                To = to ?? DateTime.UtcNow,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });

            var rowList = rows.ToList();
            var totalCount = rowList.FirstOrDefault()?.total_count ?? 0;

            // Recarrega as entidades completas via EF Core com os IDs retornados pelo Dapper
            var ids = rowList.Select(r => (Guid)r.id).ToList();
            var items = await _context.Transactions
                .Include(t => t.FraudDecision)
                .Where(t => ids.Contains(t.Id))
                .ToListAsync(ct);

            return new PagedRepositoryResult<Transaction>(items, (int)totalCount);
        }
    }
}
