using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CodingMilitia.CastleDynamicProxySample.Caching.CacheKeyCreation.Configuration
{
    public class ConfigurationBasedCacheKeyCreationStrategy : ICacheKeyCreationStrategy
    {
        private readonly ILogger<ConfigurationBasedCacheKeyCreationStrategy> _logger;
        private readonly IDictionary<string, Func<object[], string>> _customCacheKeyGenerators;

        public ConfigurationBasedCacheKeyCreationStrategy(IDictionary<string, Func<object[], string>> customCacheKeyGenerators, ILoggerFactory loggerFactory)
        {
            _customCacheKeyGenerators = customCacheKeyGenerators ?? new Dictionary<string, Func<object[], string>>();
            _logger = loggerFactory?.CreateLogger<ConfigurationBasedCacheKeyCreationStrategy>();

        }

        public string Create(string methodId, IInvocation invocation)
        {
            if (_customCacheKeyGenerators != null && !string.IsNullOrWhiteSpace(methodId) &&
                   _customCacheKeyGenerators.ContainsKey(methodId))
            {
                var cacheKey = _customCacheKeyGenerators[methodId](invocation.Arguments);
                _logger?.LogDebug($"Fetched configured cache key: \"{cacheKey}\"");
                return cacheKey;
            }
            return null;
        }
    }
}
