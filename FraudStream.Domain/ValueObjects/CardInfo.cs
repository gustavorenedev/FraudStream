using FraudStream.Domain.Exceptions;

namespace FraudStream.Domain.ValueObjects
{
    /// <summary>
    /// Informações do cartão associado à transação.
    /// Nunca armazena o número completo — apenas os últimos 4 dígitos (PCI-DSS).
    /// </summary>
    public sealed record CardInfo
    {
        public string CardId { get; }
        public string LastFourDigits { get; }
        public DateTime IssuedAt { get; }

        private CardInfo() { } // EF Core

        public CardInfo(string cardId, string lastFourDigits, DateTime issuedAt)
        {
            if (string.IsNullOrWhiteSpace(cardId))
                throw new DomainException("CardId não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(lastFourDigits) || lastFourDigits.Length != 4 || !lastFourDigits.All(char.IsDigit))
                throw new DomainException("Os últimos 4 dígitos do cartão são inválidos.");

            if (issuedAt > DateTime.UtcNow)
                throw new DomainException("A data de emissão do cartão não pode ser futura.");

            CardId = cardId;
            LastFourDigits = lastFourDigits;
            IssuedAt = issuedAt;
        }

        /// <summary>Cartão emitido há menos de N dias — usado pela regra NewCard (RG-07).</summary>
        public bool IsNewCard(int thresholdDays = 7)
            => (DateTime.UtcNow - IssuedAt).TotalDays < thresholdDays;

        public string MaskedNumber => $"**** **** **** {LastFourDigits}";

        public override string ToString() => MaskedNumber;
    }
}
