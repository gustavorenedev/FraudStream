using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Infrastructure.Rules.Rules
{
    public sealed class NewCardRule
    {
        private readonly FraudRule _config;

        public NewCardRule(FraudRule config) => _config = config;

        public RuleResult Evaluate(Transaction transaction)
        {
            var threshold = _config.Threshold ?? 1000m;
            var isNewCard = transaction.Card.IsNewCard(thresholdDays: 7);
            var isHighValue = transaction.Amount.IsHighValue(threshold);

            return isNewCard && isHighValue
                ? RuleResult.Hit(RuleType.NewCard, _config.Score,
                    $"Cartão emitido em {transaction.Card.IssuedAt:d} (menos de 7 dias) com valor {transaction.Amount}.")
                : RuleResult.Miss(RuleType.NewCard);
        }
    }
}
