using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Linq;

namespace CodingMilitia.CastleDynamicProxySample.Caching
{
    public class CacheInterceptor : IInterceptor
    {
        private readonly ICache _cacheProvider;
        private readonly ILogger<CacheInterceptor> _logger;
        private readonly ICacheKeyCreationStrategy _cacheKeyCreationStrategy;
        private readonly IConfigurationGetter _configurationGetter;
        private readonly TimeSpan _defaultTtl;

        public CacheInterceptor(ICache cacheProvider, 
                                ILoggerFactory loggerFactory,
                                ICacheKeyCreationStrategy cacheKeyCreationStrategy,
                                IConfigurationGetter configurationGetter,
                                TimeSpan defaultTtl)
        {
            ThrowIfNoCacheProvider(cacheProvider);
            ThrowIfNoCacheKeyCreationStrategy(cacheKeyCreationStrategy);

            _cacheProvider = cacheProvider;
            _logger = loggerFactory?.CreateLogger<CacheInterceptor>();
            _cacheKeyCreationStrategy = cacheKeyCreationStrategy;
            _configurationGetter = configurationGetter;
            _defaultTtl = defaultTtl;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                _logger?.LogDebug("Enter interceptor for {0}.{1} ", invocation.TargetType, invocation.Method.Name);
                var config = _configurationGetter.Get(invocation);
                var cacheKey = config.UseCache ? _cacheKeyCreationStrategy.Create(config.MethodId, invocation) : null;
                object value;
                if (config.UseCache && TryGetFromCache(cacheKey, out value))
                {
                    invocation.ReturnValue = value;
                    return;
                }
                invocation.Proceed();
                value = invocation.ReturnValue;
                AddToCache(cacheKey, config, value);
            }
            finally
            {
                _logger?.LogDebug("Exit interceptor for {0}.{1} ", invocation.TargetType, invocation.Method.Name);
            }
        }

        private static void ThrowIfNoCacheProvider(ICache cacheProvider)
        {
            if (cacheProvider == null)
            {
                throw new ArgumentException($"\"{nameof(cacheProvider)}\" is mandatory.");
            }
        }

        private static void ThrowIfNoCacheKeyCreationStrategy(ICacheKeyCreationStrategy cacheKeyCreationStrategy)
        {
            if (cacheKeyCreationStrategy == null)
            {
                throw new ArgumentException($"\"{nameof(cacheKeyCreationStrategy)}\" is mandatory.");
            }
        }

        public bool TryGetFromCache(string cacheKey, out object cached)
        {
            var cachedValue = _cacheProvider.Get(cacheKey);
            var isInCache = cachedValue.HasValue;
            cached = isInCache ? cachedValue.Value : null;
            return isInCache;
        }

        public void AddToCache(string cacheKey, MethodCacheConfiguration config, object toCache)
        {
            //if there is no config attribute, then no cache is used, return immediately
            if (!config.UseCache)
                return;

            //if the return is null it's only cached if explicitly indicated in the attribute CacheNullValues
            if (!config.CacheNullValues && toCache == null)
                return;

            //if the return is an empty collection it's only cached if explicitly indicated in the attribute CacheEmptyCollectionValues
            if (!config.CacheEmptyCollectionValues && toCache is IEnumerable && !CollectionHasElements((IEnumerable)toCache))
                return;

            var ttl = config.Ttl ?? _defaultTtl;

            _cacheProvider.Add(cacheKey, toCache, ttl);
        }

        private static bool CollectionHasElements(IEnumerable collection)
        {
            return collection.Cast<object>().Any();
        }
    }
}
