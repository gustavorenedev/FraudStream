using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FraudStream.Domain.Entities;

namespace FraudStream.Infrastructure.Persistence.Configurations
{
    public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("id");

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .HasColumnName("updated_at");

            builder.Property(t => t.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            // ── TransactionId (Value Object) ──────────────────────────
            builder.OwnsOne(t => t.TransactionId, tid =>
            {
                tid.Property(x => x.Value)
                    .HasColumnName("transaction_id")
                    .HasMaxLength(32)
                    .IsRequired();

                tid.HasIndex(x => x.Value).IsUnique();
            });

            // ── Money (Value Object) ──────────────────────────────────
            builder.OwnsOne(t => t.Amount, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("numeric(18,2)")
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // ── CardInfo (Value Object) ───────────────────────────────
            builder.OwnsOne(t => t.Card, card =>
            {
                card.Property(x => x.CardId)
                    .HasColumnName("card_id")
                    .HasMaxLength(64)
                    .IsRequired();

                card.Property(x => x.LastFourDigits)
                    .HasColumnName("card_last_four")
                    .HasMaxLength(4)
                    .IsRequired();

                card.Property(x => x.IssuedAt)
                    .HasColumnName("card_issued_at")
                    .IsRequired();
            });

            // ── DeviceInfo (Value Object) ─────────────────────────────
            builder.OwnsOne(t => t.Device, device =>
            {
                device.Property(x => x.DeviceId)
                    .HasColumnName("device_id")
                    .HasMaxLength(128)
                    .IsRequired();

                device.Property(x => x.Country)
                    .HasColumnName("country")
                    .HasMaxLength(2)
                    .IsRequired();

                device.Property(x => x.IpAddress)
                    .HasColumnName("ip_address")
                    .HasMaxLength(45);
            });

            // ── MerchantInfo (Value Object) ───────────────────────────
            builder.OwnsOne(t => t.Merchant, merchant =>
            {
                merchant.Property(x => x.MerchantId)
                    .HasColumnName("merchant_id")
                    .HasMaxLength(64)
                    .IsRequired();

                merchant.Property(x => x.Name)
                    .HasColumnName("merchant_name")
                    .HasMaxLength(200)
                    .IsRequired();

                merchant.Property(x => x.Category)
                    .HasColumnName("merchant_category")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // ── Relacionamento com FraudDecision ──────────────────────
            builder.HasOne(t => t.FraudDecision)
                .WithOne()
                .HasForeignKey<Transaction>(t => t.FraudDecisionId)
                .OnDelete(DeleteBehavior.SetNull);

            // ── Índices para queries frequentes ──────────────────────
            builder.HasIndex(t => t.Status)
                .HasDatabaseName("ix_transactions_status");

            builder.HasIndex(t => t.CreatedAt)
                .HasDatabaseName("ix_transactions_created_at");
        }
    }
}
