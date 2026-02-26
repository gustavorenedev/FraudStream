using FraudStream.Domain.Enums;
using FraudStream.Domain.Exceptions;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Domain.Entities
{
    /// <summary>
    /// Representa a decisão gerada pelo FraudRuleEngine para uma transação.
    /// Entidade filha do agregado Transaction — nunca persiste sem Transaction.
    /// Imutável após criação: decisões não são alteradas, apenas auditadas.
    /// </summary>
    public sealed class FraudDecision : BaseEntity
    {
        // ── Relacionamento ─────────────────────────────────────────────────────────
        public string TransactionId { get; private set; } = default!;

        // ── Resultado da análise ───────────────────────────────────────────────────
        public DecisionStatus Status { get; private set; }
        public ScoreBreakdown Score { get; private set; } = default!;

        // ── Metadados do Engine ────────────────────────────────────────────────────
        public string EngineVersion { get; private set; } = default!;
        public DateTime AnalyzedAt { get; private set; }

        // ── Revisão Manual (opcional) ──────────────────────────────────────────────
        public string? ReviewedBy { get; private set; }
        public DateTime? ReviewedAt { get; private set; }
        public string? ReviewNotes { get; private set; }

        private FraudDecision() { } // EF Core

        /// <summary>
        /// Factory method — cria uma decisão a partir do resultado do engine.
        /// </summary>
        public static FraudDecision Create(
            string transactionId,
            ScoreBreakdown score,
            string engineVersion)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new DomainException("TransactionId é obrigatório para criar uma FraudDecision.");

            ArgumentNullException.ThrowIfNull(score);

            if (string.IsNullOrWhiteSpace(engineVersion))
                throw new DomainException("EngineVersion é obrigatório para rastreabilidade.");

            return new FraudDecision
            {
                TransactionId = transactionId,
                Status = score.ResolveDecision(),
                Score = score,
                EngineVersion = engineVersion,
                AnalyzedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Registra uma revisão manual feita por um analista.
        /// Só pode ser aplicada em decisões com status Review.
        /// </summary>
        public void ApplyManualReview(string analystId, DecisionStatus newStatus, string notes)
        {
            if (Status != DecisionStatus.Review)
                throw new DomainException($"Revisão manual só pode ser aplicada em decisões com status Review. Status atual: {Status}.");

            if (newStatus == DecisionStatus.Review || newStatus == DecisionStatus.Pending)
                throw new DomainException($"O status resultante de uma revisão manual deve ser Approved, Blocked ou FraudConfirmed.");

            if (string.IsNullOrWhiteSpace(analystId))
                throw new DomainException("O identificador do analista é obrigatório.");

            if (string.IsNullOrWhiteSpace(notes))
                throw new DomainException("As notas de revisão são obrigatórias para fins de auditoria.");

            Status = newStatus;
            ReviewedBy = analystId;
            ReviewedAt = DateTime.UtcNow;
            ReviewNotes = notes;
            SetUpdatedAt();
        }

        // ── Queries de domínio ─────────────────────────────────────────────────────
        public bool IsFraud => Status is DecisionStatus.Blocked or DecisionStatus.FraudConfirmed;
        public bool WasManuallyReviewed => ReviewedBy is not null;
        public int TotalScore => Score.TotalScore;

        public override string ToString()
            => $"Decision={Status} Score={TotalScore} Engine={EngineVersion} At={AnalyzedAt:u}";
    }
}
