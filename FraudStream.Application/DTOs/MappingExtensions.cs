using FraudStream.Domain.Entities;
using FraudStream.Domain.Interfaces;

namespace FraudStream.Application.DTOs
{
    /// <summary>
    /// Métodos de extensão para mapear entidades do Domain para DTOs.
    /// Sem AutoMapper — mapeamento explícito e rastreável.
    /// </summary>
    public static class MappingExtensions
    {
        public static TransactionDto ToDto(this Transaction transaction)
            => new(
                TransactionId: transaction.TransactionId.Value,
                Amount: transaction.Amount.Amount,
                Currency: transaction.Amount.Currency,
                CardId: transaction.Card.CardId,
                CardLastFour: transaction.Card.LastFourDigits,
                DeviceId: transaction.Device.DeviceId,
                Country: transaction.Device.Country,
                MerchantId: transaction.Merchant.MerchantId,
                MerchantName: transaction.Merchant.Name,
                Status: transaction.Status,
                CreatedAt: transaction.CreatedAt,
                Decision: transaction.FraudDecision?.ToDto()
            );

        public static FraudDecisionDto ToDto(this FraudDecision decision)
            => new(
                Status: decision.Status,
                TotalScore: decision.TotalScore,
                ScoreBreakdown: decision.Score.Results
                    .Select(r => new RuleResultDto(r.Rule.ToString(), r.Triggered, r.Score, r.Reason))
                    .ToList(),
                EngineVersion: decision.EngineVersion,
                AnalyzedAt: decision.AnalyzedAt,
                ManualReview: decision.WasManuallyReviewed
                    ? new ManualReviewDto(
                        decision.ReviewedBy!,
                        decision.Status,
                        decision.ReviewNotes!,
                        decision.ReviewedAt!.Value)
                    : null
            );

        public static FraudSummaryDto ToDto(this FraudSummary summary)
            => new(
                Total: summary.Total,
                Approved: summary.Approved,
                Review: summary.Review,
                Blocked: summary.Blocked,
                FraudConfirmed: summary.FraudConfirmed,
                FraudRatePercent: summary.FraudRatePercent,
                From: summary.From,
                To: summary.To
            );
    }
}
