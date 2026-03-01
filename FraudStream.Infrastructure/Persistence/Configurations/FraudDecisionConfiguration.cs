using FraudStream.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Infrastructure.Persistence.Configurations
{
    public sealed class FraudDecisionConfiguration : IEntityTypeConfiguration<FraudDecision>
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = false  // campos privados ignorados — usamos propriedades com [JsonInclude]
        };

        public void Configure(EntityTypeBuilder<FraudDecision> builder)
        {
            builder.ToTable("fraud_decisions");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasColumnName("id");

            builder.Property(d => d.TransactionId)
                .HasColumnName("transaction_id")
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(d => d.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(d => d.EngineVersion)
                .HasColumnName("engine_version")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(d => d.AnalyzedAt)
                .HasColumnName("analyzed_at")
                .IsRequired();

            builder.Property(d => d.ReviewedBy)
                .HasColumnName("reviewed_by")
                .HasMaxLength(64);

            builder.Property(d => d.ReviewedAt)
                .HasColumnName("reviewed_at");

            builder.Property(d => d.ReviewNotes)
                .HasColumnName("review_notes")
                .HasMaxLength(1000);

            builder.Property(d => d.CreatedAt)
                .HasColumnName("created_at");

            builder.Property(d => d.UpdatedAt)
                .HasColumnName("updated_at");

            // ScoreBreakdown serializado como JSONB no PostgreSQL
            // Evita tabela extra de RuleResults — o breakdown é imutável após criação
            builder.Property(d => d.Score)
                .HasColumnName("score_breakdown")
                .HasColumnType("jsonb")
                .HasConversion(
                    score => JsonSerializer.Serialize(score, JsonOptions),
                    json => JsonSerializer.Deserialize<ScoreBreakdown>(json, JsonOptions)!)
                .IsRequired();

            builder.HasIndex(d => d.TransactionId)
                .HasDatabaseName("ix_fraud_decisions_transaction_id")
                .IsUnique();

            builder.HasIndex(d => d.Status)
                .HasDatabaseName("ix_fraud_decisions_status");

            builder.HasIndex(d => d.AnalyzedAt)
                .HasDatabaseName("ix_fraud_decisions_analyzed_at");
        }
    }
}
