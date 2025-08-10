using DynamicConfig.Repositories.Abstract;
using DynamicConfig.Repositories.Concrete;
using DynamicConfig.Services.Abstract;
using DynamicConfig.Services.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using static DynamicConfig.ConfigurationReader;


namespace DynamicConfig
{

    public class ConfigurationReader: IConfigurationReader
    {
        private readonly string _applicationName;
        private readonly string _connectionString;
        private readonly int _refreshTimerIntervalInMs;
        private readonly ApplicationDbContext _ctx;
        private readonly IConfigRepository _repo;
        private readonly IConfigService _service;

        private readonly IMemoryCache _memory;
        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseNpgsql(connectionString)
                    // .UseSqlServer(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .Options;
            _ctx = new ApplicationDbContext(opts);

            _repo = new ConfigRepository(_ctx);
            _service = new ConfigService(_repo);

            _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _refreshTimerIntervalInMs = refreshTimerIntervalInMs;
            _memory = new MemoryCache(new MemoryCacheOptions());
            Initialize();
        }

        private void Initialize()
        {

            Console.WriteLine($"ConfigurationReader initialized for application: {_applicationName} with refresh interval: {_refreshTimerIntervalInMs} ms");
        }
        private static string BuildCacheKey( string app, string key , string cacheName = "cfg")
            => $"{cacheName}:{app}:{key}";


        public T GetValue<T>(string key)
        {
            var cfgCacheKey = BuildCacheKey(_applicationName, key);
            var snapshotCacheKey = BuildCacheKey(_applicationName, key, "snapshot");

            if (_memory.TryGetValue(cfgCacheKey, out var value))
            {
                if (value != null)
                    return (T)value;
                return default!;
            }
            try
            {
                var val = _service.GetValue<T>(key, _applicationName);

                var entryOpts = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(_refreshTimerIntervalInMs)
                };

                _memory.Set(cfgCacheKey, val, entryOpts);
                _memory.Set(snapshotCacheKey, val);
                return val;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving value for key '{key}': {ex.Message}");
                if (_memory.TryGetValue(snapshotCacheKey, out var snapshotValue))
                {
                    if (snapshotValue != null)
                        return (T)snapshotValue;

                }
            }
            return default!;
        }
    }
}
