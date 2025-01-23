using Romb.Application.Dtos;
using StackExchange.Redis;
using System.Text.Json;

namespace Romb.Application.Services;

public class RedisService : IRedisService
{
    private readonly IDatabase _redisDatabase;
    private readonly ILogger<RedisService> _logger;

    private const string ServiceName = nameof(RedisService);

    public RedisService(IDatabase redisDatabase, ILogger<RedisService> logger)
    {
        _redisDatabase = redisDatabase;
        _logger = logger;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("[{ServiceName}]: Attempt to set cache with empty key.", ServiceName);

            throw new ArgumentException("Key cannot be empty.");
        }

        try
        {
            var jsonData = JsonSerializer.Serialize(value);

            await _redisDatabase.StringSetAsync(key, jsonData, expiry);

            _logger.LogInformation("[{ServiceName}]: Cache set for key {Key} with expiry {Expiry}.", ServiceName, key, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{ServiceName}]: Failed to set cache for key {Key}.", ServiceName, key);

            throw new RedisException("Cache write error.", ex);
        }
    }

    public async Task<T> GetAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("[{ServiceName}]: Attempt to set cache with empty key.", ServiceName);

            throw new ArgumentException("Key cannot be empty.");
        }

        try
        {
            var value = await _redisDatabase.StringGetAsync(key);

            if (!value.HasValue)
            {
                _logger.LogInformation("[{ServiceName}]: Couldn't get the value with key: {Key}.", ServiceName, key);

                return default;
            }

            _logger.LogInformation("[{ServiceName}]: Cache received successfully for key: {Key}", ServiceName, key);

            return JsonSerializer.Deserialize<T>(value);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{ServiceName}]: Redis get failed for key: {Key}.", ServiceName, key);

            throw new RedisException("Cache read error.", ex);
        }
    }

    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("[{ServiceName}]: Attempt to set cache with empty key.", ServiceName);

            throw new ArgumentException("Key cannot be empty.");
        }

        try
        {
            await _redisDatabase.KeyDeleteAsync(key);

            _logger.LogInformation("[{ServiceName}]: Cache removed for key: {Key}", ServiceName, key);
        }
        catch (Exception ex)
        {
            _logger.LogError("[{ServiceName}]: Failed to remove cache for key: {Key}", ServiceName, key);

            throw new RedisException("Cache deletion error.", ex);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        try
        {
            return await _redisDatabase.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{ServiceName}]: Failed to check existence for key {Key}", ServiceName, key);
            return false;
        }
    }
}
