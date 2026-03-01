using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.Interfaces;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Infrastructure.Rules.Rules
{
    public sealed class VelocityRule
    {
        private readonly FraudRule _config;
        private readonly IVelocityCache _cache;

        public VelocityRule(FraudRule config, IVelocityCache cache)
        {
            _config = config;
            _cache = cache;
        }

        public async Task<RuleResult> EvaluateAsync(Transaction transaction, CancellationToken ct)
        {
            var threshold = (int)(_config.Threshold ?? 5);
            var count = await _cache.IncrementAndGetAsync(transaction.Card.CardId, ct);

            return count > threshold
                ? RuleResult.Hit(RuleType.Velocity, _config.Score,
                    $"Cartão realizou {count} transações nos últimos 60s. Limite: {threshold}.")
                : RuleResult.Miss(RuleType.Velocity);
        }
    }
}
