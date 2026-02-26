using FraudStream.Domain.Enums;
using FraudStream.Domain.Exceptions;

namespace FraudStream.Domain.ValueObjects
{
    /// <summary>
    /// Resultado da avaliação de uma regra individual de fraude.
    /// Compõe o ScoreBreakdown da FraudDecision.
    /// </summary>
    public sealed record RuleResult
    {
        public RuleType Rule { get; }
        public bool Triggered { get; }
        public int Score { get; }
        public string Reason { get; }

        private RuleResult() { } // EF Core

        private RuleResult(RuleType rule, bool triggered, int score, string reason)
        {
            if (score < 0)
                throw new DomainException($"Score da regra {rule} não pode ser negativo.");

            Rule = rule;
            Triggered = triggered;
            Score = triggered ? score : 0;
            Reason = reason;
        }

        /// <summary>Cria um resultado positivo: regra disparou e adiciona score.</summary>
        public static RuleResult Hit(RuleType rule, int score, string reason)
            => new(rule, triggered: true, score, reason);

        /// <summary>Cria um resultado negativo: regra não disparou, score zero.</summary>
        public static RuleResult Miss(RuleType rule)
            => new(rule, triggered: false, score: 0, reason: "Regra não ativada.");

        public override string ToString()
            => Triggered ? $"[HIT] {Rule}: +{Score} pts — {Reason}" : $"[MISS] {Rule}";
    }
}
