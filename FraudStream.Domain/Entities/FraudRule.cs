using FraudStream.Domain.Enums;
using FraudStream.Domain.Exceptions;

namespace FraudStream.Domain.Entities
{
    /// <summary>
    /// Configuração persistida de uma regra de fraude.
    /// Permite alterar score e threshold sem redeployment — hot-reload via banco ou JSON.
    /// </summary>
    public sealed class FraudRule : BaseEntity
    {
        public RuleType Type { get; private set; }
        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public int Score { get; private set; }
        public decimal? Threshold { get; private set; }
        public bool IsEnabled { get; private set; }
        public string EngineVersion { get; private set; } = default!;

        private FraudRule() { } // EF Core

        public static FraudRule Create(
            RuleType type,
            string name,
            string description,
            int score,
            string engineVersion,
            decimal? threshold = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome da regra não pode ser vazio.");

            if (score <= 0)
                throw new DomainException($"O score da regra {type} deve ser maior que zero.");

            return new FraudRule
            {
                Type = type,
                Name = name,
                Description = description,
                Score = score,
                Threshold = threshold,
                IsEnabled = true,
                EngineVersion = engineVersion
            };
        }

        public void UpdateScore(int newScore)
        {
            if (newScore <= 0)
                throw new DomainException("Score deve ser maior que zero.");

            Score = newScore;
            SetUpdatedAt();
        }

        public void UpdateThreshold(decimal newThreshold)
        {
            if (newThreshold <= 0)
                throw new DomainException("Threshold deve ser maior que zero.");

            Threshold = newThreshold;
            SetUpdatedAt();
        }

        public void Enable()
        {
            IsEnabled = true;
            SetUpdatedAt();
        }

        public void Disable()
        {
            IsEnabled = false;
            SetUpdatedAt();
        }

        public override string ToString()
            => $"[{(IsEnabled ? "ON" : "OFF")}] {Name} — Score: {Score} | Threshold: {Threshold?.ToString() ?? "N/A"}";
    }
}
