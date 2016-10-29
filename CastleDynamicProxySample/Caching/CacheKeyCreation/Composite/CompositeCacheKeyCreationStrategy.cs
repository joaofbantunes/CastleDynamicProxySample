using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingMilitia.CastleDynamicProxySample.Caching.CacheKeyCreation.Composite
{
    public class CompositeCacheKeyCreationStrategy : ICacheKeyCreationStrategy
    {
        private readonly ILogger<CompositeCacheKeyCreationStrategy> _logger;
        private readonly IEnumerable<ICacheKeyCreationStrategy> _strategies;


        public CompositeCacheKeyCreationStrategy(ILoggerFactory loggerFactory, params ICacheKeyCreationStrategy[] strategies)
        {
            _logger = loggerFactory?.CreateLogger<CompositeCacheKeyCreationStrategy>();
            if (strategies == null || !strategies.Any())
            {
                throw new ArgumentException("No strategies were defined.", nameof(strategies));
            }
            _strategies = strategies;
        }

        public string Create(string methodId, IInvocation invocation)
        {
            string cacheKey = null;
            foreach (var strategy in _strategies)
            {
                cacheKey = strategy.Create(methodId, invocation);
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    _logger?.LogDebug($"Going with strategy of type \"{strategy.GetType().Name}\", created cache key \"{cacheKey}\"");
                    break;
                }
            }
            return cacheKey;
        }
    }
}
