using FraudStream.Domain.Enums;
using FraudStream.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace FraudStream.Domain.ValueObjects
{
    /// <summary>
    /// Agregação de todos os RuleResults gerados pelo FraudRuleEngine.
    /// Calcula o score total e determina o DecisionStatus final.
    /// </summary>
    public sealed record ScoreBreakdown
    {
        // JsonInclude expõe a propriedade pública para o serializador
        // JsonPropertyName garante o nome correto no JSONB
        [JsonInclude]
        [JsonPropertyName("results")]
        public IReadOnlyList<RuleResult> Results { get; private set; } = [];

        [JsonIgnore]
        public int TotalScore => Results.Sum(r => r.Score);

        [JsonIgnore]
        public IEnumerable<RuleResult> TriggeredRules => Results.Where(r => r.Triggered);

        // Construtor para o System.Text.Json — recebe a lista diretamente pelo nome da propriedade
        [JsonConstructor]
        public ScoreBreakdown(IReadOnlyList<RuleResult> results)
        {
            if (results is null || results.Count == 0)
                throw new DomainException("ScoreBreakdown deve conter ao menos um resultado.");

            Results = results;
        }

        // Construtor de domínio — aceita IEnumerable e converte para lista imutável
        public ScoreBreakdown(IEnumerable<RuleResult> results)
            : this((results ?? throw new DomainException("Lista de resultados não pode ser nula."))
                  .ToList()
                  .AsReadOnly())
        { }

        public DecisionStatus ResolveDecision() => TotalScore switch
        {
            <= 30 => DecisionStatus.Approved,
            > 30 and <= 60 => DecisionStatus.Review,
            > 60 and <= 100 => DecisionStatus.Blocked,
            _ => DecisionStatus.FraudConfirmed
        };

        public override string ToString()
            => $"TotalScore={TotalScore} | Decision={ResolveDecision()} | " +
               $"TriggeredRules=[{string.Join(", ", TriggeredRules.Select(r => r.Rule))}]";
    }
}
