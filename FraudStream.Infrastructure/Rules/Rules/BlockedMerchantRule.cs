using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;

namespace FraudStream.Infrastructure.Rules.Rules
{
    public sealed class BlockedMerchantRule
    {
        private readonly FraudRule _config;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "blocked_merchants";

        public BlockedMerchantRule(FraudRule config, IMemoryCache cache)
        {
            _config = config;
            _cache = cache;
        }

        public RuleResult Evaluate(Transaction transaction)
        {
            // Lista carregada e cacheada pelo DependencyInjection na inicialização
            var blockedMerchants = _cache.Get<HashSet<string>>(CacheKey) ?? [];

            return blockedMerchants.Contains(transaction.Merchant.MerchantId)
                ? RuleResult.Hit(RuleType.BlockedMerchant, _config.Score,
                    $"Merchant {transaction.Merchant.MerchantId} ({transaction.Merchant.Name}) está na lista de bloqueados.")
                : RuleResult.Miss(RuleType.BlockedMerchant);
        }
    }
}
