using FraudStream.Domain.Interfaces;
using StackExchange.Redis;

namespace FraudStream.Infrastructure.Cache
{
    public sealed class RedisVelocityCache : IVelocityCache
    {
        private readonly IDatabase _redis;
        private const int WindowSeconds = 60;

        public RedisVelocityCache(IConnectionMultiplexer redis)
            => _redis = redis.GetDatabase();

        public async Task<int> IncrementAndGetAsync(string cardId, CancellationToken ct = default)
        {
            var key = $"velocity:card:{cardId}";

            // INCR atômico — sem race condition mesmo com múltiplos workers
            var count = await _redis.StringIncrementAsync(key);

            // Define TTL apenas na primeira transação da janela
            if (count == 1)
                await _redis.KeyExpireAsync(key, TimeSpan.FromSeconds(WindowSeconds));

            return (int)count;
        }

        public async Task<int> GetCountAsync(string cardId, CancellationToken ct = default)
        {
            var key = $"velocity:card:{cardId}";
            var value = await _redis.StringGetAsync(key);
            return value.HasValue ? (int)value : 0;
        }
    }
}
