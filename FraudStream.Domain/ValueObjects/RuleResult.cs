using FraudStream.Domain.Enums;
using FraudStream.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace FraudStream.Domain.ValueObjects
{
    /// <summary>
    /// Resultado da avaliação de uma regra individual de fraude.
    /// Compõe o ScoreBreakdown da FraudDecision.
    /// </summary>
    public sealed record RuleResult
    {
        [JsonPropertyName("rule")]
        public RuleType Rule { get; init; }

        [JsonPropertyName("triggered")]
        public bool Triggered { get; init; }

        [JsonPropertyName("score")]
        public int Score { get; init; }

        [JsonPropertyName("reason")]
        public string Reason { get; init; } = string.Empty;

        // JsonConstructor instrui o serializador a usar este construtor na desserialização
        [JsonConstructor]
        public RuleResult(RuleType rule, bool triggered, int score, string reason)
        {
            if (score < 0)
                throw new DomainException($"Score da regra {rule} não pode ser negativo.");

            Rule = rule;
            Triggered = triggered;
            Score = triggered ? score : 0;
            Reason = reason;
        }

        public static RuleResult Hit(RuleType rule, int score, string reason)
            => new(rule, triggered: true, score, reason);

        public static RuleResult Miss(RuleType rule)
            => new(rule, triggered: false, score: 0, reason: "Regra não ativada.");

        public override string ToString()
            => Triggered
                ? $"[HIT] {Rule}: +{Score} pts — {Reason}"
                : $"[MISS] {Rule}";
    }
}
