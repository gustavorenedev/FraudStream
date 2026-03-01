using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Infrastructure.Rules.Rules
{
    public sealed class SuspiciousHourRule
    {
        private readonly FraudRule _config;

        public SuspiciousHourRule(FraudRule config) => _config = config;

        public RuleResult Evaluate(Transaction transaction)
        {
            var hour = transaction.CreatedAt.ToUniversalTime().Hour;
            var isSuspicious = hour is >= 0 and < 5;

            return isSuspicious
                ? RuleResult.Hit(RuleType.SuspiciousHour, _config.Score,
                    $"Transação realizada às {hour:D2}h UTC — dentro da janela de risco (00h–05h).")
                : RuleResult.Miss(RuleType.SuspiciousHour);
        }
    }
}
