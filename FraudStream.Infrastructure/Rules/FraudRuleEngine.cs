using FraudStream.Domain.Entities;
using FraudStream.Domain.Enums;
using FraudStream.Domain.Interfaces;
using FraudStream.Domain.ValueObjects;
using FraudStream.Infrastructure.Rules.Rules;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FraudStream.Infrastructure.Rules
{
    public sealed class FraudRuleEngine : IFraudRuleEngine
    {
        private readonly IFraudRuleRepository _ruleRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IVelocityCache _velocityCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<FraudRuleEngine> _logger;

        public FraudRuleEngine(
            IFraudRuleRepository ruleRepository,
            ITransactionRepository transactionRepository,
            IVelocityCache velocityCache,
            IMemoryCache memoryCache,
            ILogger<FraudRuleEngine> logger)
        {
            _ruleRepository = ruleRepository;
            _transactionRepository = transactionRepository;
            _velocityCache = velocityCache;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<ScoreBreakdown> EvaluateAsync(Transaction transaction, CancellationToken ct = default)
        {
            var rules = (await _ruleRepository.GetAllEnabledAsync(ct))
                .ToDictionary(r => r.Type);

            // Monta as tasks para cada regra habilitada — execução paralela
            var tasks = new List<Task<RuleResult>>();

            if (rules.TryGetValue(RuleType.HighValue, out var hvRule))
                tasks.Add(Task.FromResult(new HighValueRule(hvRule).Evaluate(transaction)));

            if (rules.TryGetValue(RuleType.Velocity, out var velRule))
                tasks.Add(new VelocityRule(velRule, _velocityCache).EvaluateAsync(transaction, ct));

            if (rules.TryGetValue(RuleType.UnusualCountry, out var ucRule))
                tasks.Add(new UnusualCountryRule(ucRule, _transactionRepository).EvaluateAsync(transaction, ct));

            if (rules.TryGetValue(RuleType.SuspiciousHour, out var shRule))
                tasks.Add(Task.FromResult(new SuspiciousHourRule(shRule).Evaluate(transaction)));

            if (rules.TryGetValue(RuleType.NewDevice, out var ndRule))
                tasks.Add(new NewDeviceRule(ndRule, _transactionRepository).EvaluateAsync(transaction, ct));

            if (rules.TryGetValue(RuleType.BlockedMerchant, out var bmRule))
                tasks.Add(Task.FromResult(new BlockedMerchantRule(bmRule, _memoryCache).Evaluate(transaction)));

            if (rules.TryGetValue(RuleType.NewCard, out var ncRule))
                tasks.Add(Task.FromResult(new NewCardRule(ncRule).Evaluate(transaction)));

            var results = await Task.WhenAll(tasks);

            var breakdown = new ScoreBreakdown(results);

            _logger.LogInformation(
                "Engine avaliou transação {TransactionId}. Score={Score} Decision={Decision}",
                transaction.TransactionId.Value,
                breakdown.TotalScore,
                breakdown.ResolveDecision());

            return breakdown;
        }
    }
}
