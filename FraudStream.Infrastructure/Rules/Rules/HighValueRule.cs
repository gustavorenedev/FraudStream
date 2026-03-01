using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Infrastructure.Rules.Rules
{
    public sealed class HighValueRule
    {
        private readonly FraudRule _config;

        public HighValueRule(FraudRule config) => _config = config;

        public RuleResult Evaluate(Transaction transaction)
        {
            var threshold = _config.Threshold ?? 5000m;

            return transaction.Amount.IsHighValue(threshold)
                ? RuleResult.Hit(RuleType.HighValue, _config.Score,
                    $"Valor {transaction.Amount} ultrapassa o limite de {threshold:C}.")
                : RuleResult.Miss(RuleType.HighValue);
        }
    }
}
