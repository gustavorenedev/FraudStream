using FraudStream.Domain.Entities;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Domain.Interfaces
{
    /// <summary>
    /// Contrato do motor de regras de fraude.
    /// Implementado na Infrastructure — o domínio define o contrato, não a estratégia de avaliação.
    /// </summary>
    public interface IFraudRuleEngine
    {
        /// <summary>
        /// Avalia todas as regras habilitadas para a transação informada.
        /// Execução paralela via Task.WhenAll — cada regra é independente.
        /// </summary>
        Task<ScoreBreakdown> EvaluateAsync(Transaction transaction, CancellationToken ct = default);
    }
}
