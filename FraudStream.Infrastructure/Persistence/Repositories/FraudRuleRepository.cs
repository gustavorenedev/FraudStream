using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FraudStream.Infrastructure.Persistence.Repositories
{
    public sealed class FraudRuleRepository : IFraudRuleRepository
    {
        private readonly FraudStreamDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "fraud_rules_enabled";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

        public FraudRuleRepository(FraudStreamDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<FraudRule?> GetByTypeAsync(RuleType type, CancellationToken ct = default)
            => await _context.FraudRules.FirstOrDefaultAsync(r => r.Type == type, ct);

        public async Task<IEnumerable<FraudRule>> GetAllEnabledAsync(CancellationToken ct = default)
        {
            if (_cache.TryGetValue(CacheKey, out IEnumerable<FraudRule>? cached))
                return cached!;

            var rules = await _context.FraudRules
                .Where(r => r.IsEnabled)
                .ToListAsync(ct);

            _cache.Set(CacheKey, rules, CacheTtl);
            return rules;
        }

        public async Task AddAsync(FraudRule rule, CancellationToken ct = default)
        {
            await _context.FraudRules.AddAsync(rule, ct);
            await _context.SaveChangesAsync(ct);
            _cache.Remove(CacheKey); // invalida o cache ao adicionar
        }

        public async Task UpdateAsync(FraudRule rule, CancellationToken ct = default)
        {
            _context.FraudRules.Update(rule);
            await _context.SaveChangesAsync(ct);
            _cache.Remove(CacheKey); // invalida o cache ao atualizar
        }
    }
}
