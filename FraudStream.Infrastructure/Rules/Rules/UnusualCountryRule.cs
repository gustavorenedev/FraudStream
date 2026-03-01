using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.Interfaces;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Infrastructure.Rules.Rules
{
    public sealed class UnusualCountryRule
    {
        private readonly FraudRule _config;
        private readonly ITransactionRepository _repository;

        public UnusualCountryRule(FraudRule config, ITransactionRepository repository)
        {
            _config = config;
            _repository = repository;
        }

        public async Task<RuleResult> EvaluateAsync(Transaction transaction, CancellationToken ct)
        {
            var recentCountries = (await _repository.GetByCardIdAsync(transaction.Card.CardId, limit: 100, ct))
                .Where(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                .Select(t => t.Device.Country)
                .Distinct()
                .ToHashSet();

            // Cartão sem histórico — não dispara a regra
            if (recentCountries.Count == 0)
                return RuleResult.Miss(RuleType.UnusualCountry);

            var currentCountry = transaction.Device.Country;

            return !recentCountries.Contains(currentCountry)
                ? RuleResult.Hit(RuleType.UnusualCountry, _config.Score,
                    $"País {currentCountry} nunca visto nos últimos 30 dias. Histórico: [{string.Join(", ", recentCountries)}].")
                : RuleResult.Miss(RuleType.UnusualCountry);
        }
    }
}
