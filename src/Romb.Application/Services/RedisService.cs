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

    public async Task SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken token = default)
    {
        var operationName = nameof(SetAsync);

        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("[{ServiceName}]: Attempting to set cache with empty key.", ServiceName);

            return;
        }

        var jsonData = JsonSerializer.Serialize(value);

        _logger.LogInformation("[{ServiceName}]: Adding data to the Redis cache with key: {Key}, expiry: {Expiry}...", ServiceName, key, expiry);

        await ExecuteWithHandlingAsync(() =>
            _redisDatabase.StringSetAsync(key, jsonData, expiry).WaitAsync(token),
            key,
            operationName,
            token
        );
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken token = default)
    {
        var operationName = nameof(GetAsync);

        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("[{ServiceName}]: Attempting to get cache with empty key.", ServiceName);

            return default;
        }

        _logger.LogInformation("[{ServiceName}]: Getting cache from Redis with key: {Key}...", ServiceName, key);

        return await ExecuteWithHandlingAsync(async () =>
        {
            var value = await _redisDatabase.StringGetAsync(key).WaitAsync(token);

            if (!value.HasValue)
            {
                _logger.LogInformation("[{ServiceName}]: Couldn't get the cache with key: {Key}.", ServiceName, key);

                return default;
            }

            return JsonSerializer.Deserialize<T>(value);
        },
            key,
            operationName,
            token
        );
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        var operationName = nameof(RemoveAsync);

        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("[{ServiceName}]: Attempting to remove cache with empty key.", ServiceName);

            return;
        }

        _logger.LogInformation("[{ServiceName}]: Removing data from the Redis cache with key: {Key}...", ServiceName, key);

        await ExecuteWithHandlingAsync(() =>
            _redisDatabase.KeyDeleteAsync(key).WaitAsync(token),
            key,
            operationName,
            token
        );
    }

    private async Task<T> ExecuteWithHandlingAsync<T>(Func<Task<T>> operation, string key, string operationName, CancellationToken token = default)
    {
        try
        {
            return await operation();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("[{ServiceName}]: {OperationName} operation was cancelled.", ServiceName, operationName);

            throw;
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "[{ServiceName}]: Redis unavailable.", ServiceName);

            return default;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[{ServiceName}]: An error occurred in the {OperationName} operation with key: {Key}.", ServiceName, operationName, key);

            return default;
        }
    }

    private async Task ExecuteWithHandlingAsync(Func<Task> operation, string key, string operationName, CancellationToken token = default)
    {
        try
        {
            await operation();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("[{ServiceName}]: {OperationName} operation was cancelled.", ServiceName, operationName);

            throw;
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "[{ServiceName}]: Redis unavailable.", ServiceName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[{ServiceName}]: An error occurred in the {OperationName} operation with key: {Key}.", ServiceName, operationName, key);
        }
    }
}
