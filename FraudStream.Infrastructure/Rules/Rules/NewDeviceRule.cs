using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.Interfaces;
using FraudStream.Domain.ValueObjects;

namespace FraudStream.Infrastructure.Rules.Rules
{
    public sealed class NewDeviceRule
    {
        private readonly FraudRule _config;
        private readonly ITransactionRepository _repository;

        public NewDeviceRule(FraudRule config, ITransactionRepository repository)
        {
            _config = config;
            _repository = repository;
        }

        public async Task<RuleResult> EvaluateAsync(Transaction transaction, CancellationToken ct)
        {
            var knownDevices = (await _repository.GetByCardIdAsync(transaction.Card.CardId, limit: 200, ct))
                .Select(t => t.Device.DeviceId)
                .Distinct()
                .ToHashSet();

            if (knownDevices.Count == 0)
                return RuleResult.Miss(RuleType.NewDevice);

            var isNewDevice = !knownDevices.Contains(transaction.Device.DeviceId);

            return isNewDevice
                ? RuleResult.Hit(RuleType.NewDevice, _config.Score,
                    $"Device {transaction.Device.DeviceId} nunca utilizado por este cartão.")
                : RuleResult.Miss(RuleType.NewDevice);
        }
    }
}
