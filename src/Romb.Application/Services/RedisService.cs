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

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("[{ServiceName}]: Attempt to set cache with empty key.", ServiceName);

            throw new ArgumentException("Key cannot be empty.");
        }

        try
        {
            var jsonData = JsonSerializer.Serialize(value);

            await _redisDatabase.StringSetAsync(key, jsonData, expiry).WaitAsync(token);

            _logger.LogInformation("[{ServiceName}]: Cache set for key {Key} with expiry {Expiry}.", ServiceName, key, expiry);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[{ServiceName}]: Operation was cancelled.", ServiceName);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("[{ServiceName}]: Failed to set cache for key {Key}.", ServiceName, key);

            throw new RedisException("Cache write error.", ex);
        }
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("[{ServiceName}]: Attempt to get cache with empty key.", ServiceName);

            throw new ArgumentException("Key cannot be empty.");
        }

        try
        {
            var value = await _redisDatabase.StringGetAsync(key).WaitAsync(token);

            if (!value.HasValue)
            {
                _logger.LogInformation("[{ServiceName}]: Couldn't get the value with key: {Key}.", ServiceName, key);

                return default;
            }

            _logger.LogInformation("[{ServiceName}]: Cache received successfully for key: {Key}", ServiceName, key);

            return JsonSerializer.Deserialize<T>(value);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[{ServiceName}]: Operation was cancelled.", ServiceName);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("[{ServiceName}]: Redis get failed for key: {Key}.", ServiceName, key);

            throw new RedisException("Cache read error.", ex);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("[{ServiceName}]: Attempt to remove cache with empty key.", ServiceName);

            throw new ArgumentException("Key cannot be empty.");
        }

        try
        {
            await _redisDatabase.KeyDeleteAsync(key).WaitAsync(token);

            _logger.LogInformation("[{ServiceName}]: Cache removed for key: {Key}", ServiceName, key);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[{ServiceName}]: Operation was cancelled.", ServiceName);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("[{ServiceName}]: Failed to remove cache for key: {Key}", ServiceName, key);

            throw new RedisException("Cache deletion error.", ex);
        }
    }
}
