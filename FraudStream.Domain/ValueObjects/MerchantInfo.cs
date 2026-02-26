using FraudStream.Domain.Exceptions;

namespace FraudStream.Domain.ValueObjects
{
    /// <summary>
    /// Informações do estabelecimento que recebe o pagamento.
    /// Usado pela regra BlockedMerchant (RG-06).
    /// </summary>
    public sealed record MerchantInfo
    {
        public string MerchantId { get; }
        public string Name { get; }
        public string Category { get; }

        private MerchantInfo() { } // EF Core

        public MerchantInfo(string merchantId, string name, string category)
        {
            if (string.IsNullOrWhiteSpace(merchantId))
                throw new DomainException("MerchantId não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome do merchant não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(category))
                throw new DomainException("A categoria do merchant não pode ser vazia.");

            MerchantId = merchantId;
            Name = name;
            Category = category;
        }

        public override string ToString() => $"{Name} ({Category})";
    }
}
