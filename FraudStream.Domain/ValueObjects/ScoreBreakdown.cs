using FraudStream.Domain.Enums;
using FraudStream.Domain.Exceptions;

namespace FraudStream.Domain.ValueObjects
{
    /// <summary>
    /// Agregação de todos os RuleResults gerados pelo FraudRuleEngine.
    /// Calcula o score total e determina o DecisionStatus final.
    /// </summary>
    public sealed record ScoreBreakdown
    {
        private readonly List<RuleResult> _results;

        public IReadOnlyList<RuleResult> Results => _results.AsReadOnly();
        public int TotalScore => _results.Sum(r => r.Score);
        public IEnumerable<RuleResult> TriggeredRules => _results.Where(r => r.Triggered);

        private ScoreBreakdown() => _results = []; // EF Core

        public ScoreBreakdown(IEnumerable<RuleResult> results)
        {
            var list = results?.ToList() ?? throw new DomainException("Lista de resultados não pode ser nula.");
            if (list.Count == 0)
                throw new DomainException("ScoreBreakdown deve conter ao menos um resultado.");

            _results = list;
        }

        /// <summary>
        /// Determina o DecisionStatus com base no score total acumulado.
        /// Thresholds configuráveis — aqui representados como constantes do domínio.
        /// </summary>
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
