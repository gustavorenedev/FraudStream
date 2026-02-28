namespace FraudStream.Application.DTOs
{
    /// <summary>
    /// DTO de resumo agregado para o dashboard do Grafana.
    /// Retornado por GET /api/fraud/summary.
    /// </summary>
    public sealed record FraudSummaryDto(
        int Total,
        int Approved,
        int Review,
        int Blocked,
        int FraudConfirmed,
        decimal FraudRatePercent,
        DateTime From,
        DateTime To
    );

    /// <summary>
    /// DTO com distribuição de score por regra — para o gráfico de barras do Grafana.
    /// </summary>
    public sealed record RuleHitSummaryDto(
        string Rule,
        int TotalHits,
        int TotalScoreContributed
    );
}
