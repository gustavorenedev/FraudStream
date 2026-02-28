using FraudStream.Domain.Enums;

namespace FraudStream.Application.DTOs
{
    /// <summary>
    /// DTO de leitura de uma FraudDecision com o breakdown de score por regra.
    /// </summary>
    public sealed record FraudDecisionDto(
        DecisionStatus Status,
        int TotalScore,
        IReadOnlyList<RuleResultDto> ScoreBreakdown,
        string EngineVersion,
        DateTime AnalyzedAt,
        ManualReviewDto? ManualReview
    );

    public sealed record RuleResultDto(
        string Rule,
        bool Triggered,
        int Score,
        string Reason
    );

    public sealed record ManualReviewDto(
        string AnalystId,
        DecisionStatus FinalDecision,
        string Notes,
        DateTime ReviewedAt
    );
}
