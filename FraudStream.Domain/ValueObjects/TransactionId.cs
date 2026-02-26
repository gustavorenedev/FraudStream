using FraudStream.Domain.Exceptions;

namespace FraudStream.Domain.ValueObjects
{
    /// Identificador único de uma transação.
    /// Formato: txn_{26 chars alfanuméricos} — ex: txn_7f3a1b9c4d2e8f1a0b5c6d7e8f
    /// </summary>
    public sealed record TransactionId
    {
        public string Value { get; }

        private const string Prefix = "txn_";
        private const int SuffixLength = 26;

        private TransactionId() { } // EF Core

        public TransactionId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("TransactionId não pode ser vazio.");

            if (!value.StartsWith(Prefix) || value.Length != Prefix.Length + SuffixLength)
                throw new DomainException($"TransactionId inválido. Formato esperado: {Prefix}{{26 chars}}.");

            Value = value;
        }

        public static TransactionId New()
        {
            var suffix = Guid.NewGuid().ToString("N")[..SuffixLength];
            return new TransactionId($"{Prefix}{suffix}");
        }

        public static TransactionId From(string value) => new(value);

        public override string ToString() => Value;

        public static implicit operator string(TransactionId id) => id.Value;
    }
}
