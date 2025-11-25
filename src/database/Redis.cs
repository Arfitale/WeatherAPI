using DotNetEnv;
using StackExchange.Redis;

public class Redis
{
    private readonly ConnectionMultiplexer _muxer;

    public Redis()
    {
        _muxer = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints =
                {
                    { "redis-19964.c334.asia-southeast2-1.gce.cloud.redislabs.com", 19964 },
                },
                User = "default",
                Password =
                    Env.GetString("REDIS_PASSWORD")
                    ?? Environment.GetEnvironmentVariable("REDIS_PASSWORD"),
            }
        );
    }

    public Task<IDatabase> GetDatabaseAsync()
    {
        return Task.FromResult(_muxer.GetDatabase());
    }

    public async Task<bool> SetCacheValueAsync(string key, string value, TimeSpan expiry = default)
    {
        var db = _muxer.GetDatabase();
        return await db.StringSetAsync(key, value, expiry == default ? (TimeSpan?)null : expiry);
    }

    public async Task<RedisValue?> GetCacheValueAsync(string key)
    {
        var db = _muxer.GetDatabase();
        var value = await db.StringGetAsync(key);

        if (value.IsNull)
        {
            return null;
        }
        return value;
    }
}
