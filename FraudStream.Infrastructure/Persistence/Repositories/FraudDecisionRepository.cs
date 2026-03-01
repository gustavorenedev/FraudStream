using Dapper;
using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FraudStream.Infrastructure.Persistence.Repositories
{
    public sealed class FraudDecisionRepository : IFraudDecisionRepository
    {
        private readonly FraudStreamDbContext _context;
        private readonly IDbConnection _db;

        public FraudDecisionRepository(FraudStreamDbContext context, IDbConnection db)
        {
            _context = context;
            _db = db;
        }

        public async Task<FraudDecision?> GetByTransactionIdAsync(string transactionId, CancellationToken ct = default)
            => await _context.FraudDecisions
                .FirstOrDefaultAsync(d => d.TransactionId == transactionId, ct);

        public async Task<IEnumerable<FraudDecision>> GetByStatusAsync(DecisionStatus status, int limit = 100, CancellationToken ct = default)
            => await _context.FraudDecisions
                .Where(d => d.Status == status)
                .OrderByDescending(d => d.AnalyzedAt)
                .Take(limit)
                .ToListAsync(ct);

        public async Task<FraudSummary> GetSummaryAsync(DateTime from, DateTime to, CancellationToken ct = default)
        {
            var sql = """
            SELECT
                COUNT(*)                                                        AS total,
                COUNT(*) FILTER (WHERE status = 'Approved')                    AS approved,
                COUNT(*) FILTER (WHERE status = 'Review')                      AS review,
                COUNT(*) FILTER (WHERE status = 'Blocked')                     AS blocked,
                COUNT(*) FILTER (WHERE status = 'FraudConfirmed')              AS fraud_confirmed,
                ROUND(
                    COUNT(*) FILTER (WHERE status IN ('Blocked','FraudConfirmed'))
                    * 100.0 / NULLIF(COUNT(*), 0), 2
                )                                                               AS fraud_rate
            FROM fraud_decisions
            WHERE analyzed_at BETWEEN @From AND @To
            """;

            var result = await _db.QuerySingleAsync(sql, new { From = from, To = to });

            return new FraudSummary(
                Total: (int)result.total,
                Approved: (int)result.approved,
                Review: (int)result.review,
                Blocked: (int)result.blocked,
                FraudConfirmed: (int)result.fraud_confirmed,
                FraudRatePercent: (decimal)(result.fraud_rate ?? 0),
                From: from,
                To: to
            );
        }

        public async Task AddAsync(FraudDecision decision, CancellationToken ct = default)
        {
            await _context.FraudDecisions.AddAsync(decision, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(FraudDecision decision, CancellationToken ct = default)
        {
            _context.FraudDecisions.Update(decision);
            await _context.SaveChangesAsync(ct);
        }
    }
}
