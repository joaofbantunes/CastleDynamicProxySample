using CodingMilitia.CastleDynamicProxySample.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingMilitia.CachingSampleBenchmarksApplication.Service
{
    public class StuffServiceCacheDecorator : IStuffService
    {
        private static readonly TimeSpan _ttl = TimeSpan.FromMinutes(2);
        private readonly ICache _cache;
        private readonly ILogger<StuffServiceCacheDecorator> _logger;
        private readonly IStuffService _continuation;

        public StuffServiceCacheDecorator(ICache cache, ILoggerFactory loggerFactory, IStuffService continuation)
        {
            _cache = cache;
            _logger = loggerFactory?.CreateLogger<StuffServiceCacheDecorator>();
            _continuation = continuation;
        }

        public TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn)
        {
            try
            {
                _logger?.LogDebug("Enter decorator for {0}.{1} ", _continuation.GetType().Name, nameof(GetGenericStuff));
                string cacheKey = $"GetGenericStuff(\"{stuffId}\",\"{stuffToReturn}\")";
                var cachedValue = _cache.Get(cacheKey);
                if (cachedValue.HasValue)
                {
                    return (TOut)cachedValue.Value;
                }
                var value = _continuation.GetGenericStuff(stuffId, stuffToReturn);
                _cache.Add(cacheKey, stuffToReturn, _ttl);
                return value;
            }
            finally
            {
                _logger?.LogDebug("Exit decorator for {0}.{1} ", _continuation.GetType().Name, nameof(GetGenericStuff));
            }
        }

        public TOut GetGenericStuffGenerateKey<TOut, TIn>(TIn stuffId, TOut stuffToReturn)
        {
            throw new NotImplementedException();
        }
    }
}
