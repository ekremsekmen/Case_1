using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using StackExchange.Redis;

namespace Case_1_2.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<CacheService> _logger;

        public CacheService(
            IDistributedCache distributedCache, 
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var cachedValue = await _distributedCache.GetStringAsync(key);
                
                if (string.IsNullOrEmpty(cachedValue))
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return null;
                }

                _logger.LogDebug("Cache hit for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(cachedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions();

                if (expiration.HasValue)
                {
                    options.SetAbsoluteExpiration(expiration.Value);
                }
                else
                {
                    // Default expiration: 10 minutes
                    options.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                }

                await _distributedCache.SetStringAsync(key, serializedValue, options);
                _logger.LogDebug("Cache set for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
                _logger.LogDebug("Cache removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
            }
        }

        public async Task RemovePatternAsync(string pattern)
        {
            try
            {
                var database = _connectionMultiplexer.GetDatabase();
                var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
                
                var keys = server.Keys(pattern: pattern);
                
                foreach (var key in keys)
                {
                    await database.KeyDeleteAsync(key);
                }
                
                _logger.LogDebug("Cache pattern removed: {Pattern}", pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache pattern: {Pattern}", pattern);
            }
        }
    }
}
