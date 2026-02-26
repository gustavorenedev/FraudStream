namespace FraudStream.Domain.Interfaces
{
    /// <summary>
    /// Abstração do cache de velocidade para a regra RG-02.
    /// Implementado na Infrastructure com Redis (operação INCR atômica com TTL).
    /// O domínio não conhece Redis — apenas o contrato.
    /// </summary>
    public interface IVelocityCache
    {
        /// <summary>
        /// Incrementa o contador de transações do cartão e retorna o total no período.
        /// A janela de tempo é controlada pelo TTL configurado na infraestrutura (padrão: 60s).
        /// </summary>
        Task<int> IncrementAndGetAsync(string cardId, CancellationToken ct = default);

        /// <summary>Retorna o total atual sem incrementar — usado para consulta.</summary>
        Task<int> GetCountAsync(string cardId, CancellationToken ct = default);
    }
}
