using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;

namespace FraudStream.Domain.Interfaces
{
    /// <summary>
    /// Contrato do repositório de regras de fraude.
    /// Permite que o FraudRuleEngine carregue regras atualizadas sem restart.
    /// </summary>
    public interface IFraudRuleRepository
    {
        Task<FraudRule?> GetByTypeAsync(RuleType type, CancellationToken ct = default);
        Task<IEnumerable<FraudRule>> GetAllEnabledAsync(CancellationToken ct = default);
        Task AddAsync(FraudRule rule, CancellationToken ct = default);
        Task UpdateAsync(FraudRule rule, CancellationToken ct = default);
    }
}
