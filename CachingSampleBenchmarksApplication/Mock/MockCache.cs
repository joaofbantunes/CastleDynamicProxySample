using System;
using CodingMilitia.CastleDynamicProxySample.Caching;
using Microsoft.Extensions.Logging;

namespace CodingMilitia.CachingSampleBenchmarksApplication.Mock
{
    public class MockCache : ICache
    {
        private ILogger _logger;
        public MockCache(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<MockCache>();
        }

        public void Add(string key, object value, TimeSpan timeToLive)
        {
            _logger?.LogDebug($"\"{nameof(Add)}\" invoked with key \"{key}\", value \"{value}\", ttl \"{timeToLive}\"");
        }

        public CachedValue Get(string key)
        {
            _logger?.LogDebug($"\"{nameof(Get)}\" invoked with key \"{key}\"");
            return new CachedValue(false, null);
        }
    }
}