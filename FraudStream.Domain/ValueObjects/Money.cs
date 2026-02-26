using FraudStream.Domain.Exceptions;

namespace FraudStream.Domain.ValueObjects
{
    /// Value Object imutável — igualdade baseada em valor, não em referência.
    /// </summary>
    public sealed record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public static readonly Money Zero = new(0, "BRL");

        private Money() { } // EF Core

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new DomainException("O valor monetário não pode ser negativo.");

            if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
                throw new DomainException("Moeda inválida. Informe um código ISO 4217 de 3 letras (ex: BRL, USD).");

            Amount = Math.Round(amount, 2);
            Currency = currency.ToUpperInvariant();
        }

        public bool IsHighValue(decimal threshold) => Amount >= threshold;

        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new DomainException($"Não é possível somar valores em moedas diferentes: {a.Currency} e {b.Currency}.");

            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
