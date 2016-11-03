using Castle.DynamicProxy;
using CodingMilitia.CastleDynamicProxySample.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CodingMilitia.CastleDynamicProxySample.Caching
{
    public class CacheInterceptor : IInterceptor
    {
        private readonly ICache _cacheProvider;
        private readonly ILogger<CacheInterceptor> _logger;
        private readonly ICacheKeyCreationStrategy _cacheKeyCreationStrategy;
        private readonly IConfigurationGetter _configurationGetter;
        private readonly TimeSpan _defaultTtl;

        private readonly IDictionary<Type, MethodInfo> _taskFromResultCache;
        private readonly IDictionary<Type, MethodInfo> _awaitAndGetResultCache;

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

            _taskFromResultCache = new Dictionary<Type, MethodInfo>();
            _awaitAndGetResultCache = new Dictionary<Type, MethodInfo>();
        }

        #region validation
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
        #endregion validation

        public void Intercept(IInvocation invocation)
        {
            _logger?.LogDebug("Enter interceptor for {0}.{1} ", invocation.TargetType, invocation.Method.Name);
            var config = _configurationGetter.Get(invocation);
            var cacheKey = config.UseCache ? _cacheKeyCreationStrategy.Create(config.MethodId, invocation) : null;

            if (AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                ProceedAsync(invocation, config, cacheKey);
            }
            else
            {
                ProceedSync(invocation, config, cacheKey);
            }
        }
        #region proceed
        private void ProceedSync(IInvocation invocation, MethodCacheConfiguration config, string cacheKey)
        {
            try
            {
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
                LogExiting(invocation);
            }
        }

        private void ProceedAsync(IInvocation invocation, MethodCacheConfiguration config, string cacheKey)
        {
            object value;
            if (config.UseCache && TryGetFromCache(cacheKey, out value))
            {
                invocation.ReturnValue = GetTaskFromResultMethod(invocation).Invoke(null, new object[] { value });
                LogExiting(invocation);
                return;
            }
            invocation.Proceed();
            Action<object> addToCacheAction = (result) => AddToCache(cacheKey, config, result);
            Action logExitingAction = () => LogExiting(invocation);
            invocation.ReturnValue = GetAwaitAndGetResultMethod(invocation).Invoke(null, new object[] { invocation.ReturnValue, addToCacheAction, logExitingAction });
        }
        #endregion proceed

        #region cache provider
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
        #endregion cache provider

        #region helpers
        private static bool CollectionHasElements(IEnumerable collection)
        {
            return collection.Cast<object>().Any();
        }

        private void LogExiting(IInvocation invocation)
        {
            _logger?.LogDebug("Exit interceptor for {0}.{1} ", invocation.TargetType, invocation.Method.Name);
        }
        #endregion helpers

        #region get method helpers
        private MethodInfo GetTaskFromResultMethod(IInvocation invocation)
        {
            return GetFromCacheOrCreateMethod(_taskFromResultCache,
              invocation.Method.ReturnType.GenericTypeArguments[0],
              typeof(Task),
              nameof(Task.FromResult),
              BindingFlags.Public | BindingFlags.Static);
        }

        private MethodInfo GetAwaitAndGetResultMethod(IInvocation invocation)
        {
            return GetFromCacheOrCreateMethod(_awaitAndGetResultCache,
                invocation.Method.ReturnType.GenericTypeArguments[0],
                typeof(AsyncHelper),
                nameof(AsyncHelper.AwaitTaskWithFinallyAndGetResult),
                BindingFlags.Public | BindingFlags.Static);
        }

        private static MethodInfo GetFromCacheOrCreateMethod(IDictionary<Type, MethodInfo> cache, Type returnType, Type targetType, string methodName, BindingFlags bindingFlags)
        {
            MethodInfo genericMethod;
            if (!cache.TryGetValue(returnType, out genericMethod))
            {
                genericMethod = targetType
                            .GetMethod(methodName, bindingFlags)
                            .MakeGenericMethod(returnType);
                cache[returnType] = genericMethod;
            }
            return genericMethod;
        }
        #endregion get method helpers
    }
}
