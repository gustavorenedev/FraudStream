using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FraudStream.Infrastructure.Persistence.Configurations
{
    public sealed class FraudRuleConfiguration : IEntityTypeConfiguration<FraudRule>
    {
        public void Configure(EntityTypeBuilder<FraudRule> builder)
        {
            builder.ToTable("fraud_rules");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).HasColumnName("id");
            builder.Property(r => r.Type).HasColumnName("type").HasConversion<string>().IsRequired();
            builder.Property(r => r.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            builder.Property(r => r.Description).HasColumnName("description").HasMaxLength(500);
            builder.Property(r => r.Score).HasColumnName("score").IsRequired();
            builder.Property(r => r.Threshold).HasColumnName("threshold").HasColumnType("numeric(18,2)");
            builder.Property(r => r.IsEnabled).HasColumnName("is_enabled").IsRequired();
            builder.Property(r => r.EngineVersion).HasColumnName("engine_version").HasMaxLength(20);
            builder.Property(r => r.CreatedAt).HasColumnName("created_at");
            builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");

            builder.HasIndex(r => r.Type)
                .HasDatabaseName("ix_fraud_rules_type")
                .IsUnique();

            // Seed das 7 regras padrão — banco sobe já configurado
            builder.HasData(
                CreateRule(RuleType.HighValue, "Alto Valor", 35, threshold: 5000m),
                CreateRule(RuleType.Velocity, "Velocidade", 40, threshold: 5m),
                CreateRule(RuleType.UnusualCountry, "País Incomum", 30),
                CreateRule(RuleType.SuspiciousHour, "Horário Suspeito", 15),
                CreateRule(RuleType.NewDevice, "Novo Dispositivo", 25),
                CreateRule(RuleType.BlockedMerchant, "Merchant Bloqueado", 50),
                CreateRule(RuleType.NewCard, "Cartão Novo", 20, threshold: 1000m)
            );
        }

        private static object CreateRule(RuleType type, string name, int score, decimal? threshold = null)
            => new
            {
                Id = Guid.NewGuid(),
                Type = type,
                Name = name,
                Description = $"Regra {name} — configuração padrão.",
                Score = score,
                Threshold = threshold,
                IsEnabled = true,
                EngineVersion = "1.0.0",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = (DateTime?)null
            };
    }
}
