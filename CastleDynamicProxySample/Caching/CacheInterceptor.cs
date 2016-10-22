using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace CodingMilitia.CastleDynamicProxySample.Caching
{
    public class CacheInterceptor : IInterceptor
    {
        private readonly ICache _cacheProvider;
        private readonly ILogger<CacheInterceptor> _logger;
        private readonly IDictionary<string, Func<object[], string>> _customCacheKeyGenerators;
        private readonly IDictionary<string, TimeSpan> _cacheTtLs;
        private readonly TimeSpan? _defaultTtl;

        public CacheInterceptor(ICache cacheProvider, ILoggerFactory loggerFactory,
            IDictionary<string, Func<object[], string>> customCacheKeyGenerators,
            IDictionary<string, TimeSpan> cacheTtLs, TimeSpan? defaultTtl = null)
        {
            _cacheProvider = cacheProvider;
            _logger = loggerFactory?.CreateLogger<CacheInterceptor>();
            _customCacheKeyGenerators = customCacheKeyGenerators;
            _cacheTtLs = cacheTtLs ?? new Dictionary<string, TimeSpan>();
            _defaultTtl = defaultTtl;

            ThrowIfNoCacheProvider(cacheProvider);
            ThrowIfNoTtlsAreProvided(cacheTtLs, defaultTtl);
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                _logger?.LogDebug("Enter interceptor for {0}.{1} ", invocation.TargetType, invocation.Method.Name);
                var helper = new CacheInterceptionHelper(invocation, _cacheProvider, _logger, _customCacheKeyGenerators, _cacheTtLs, _defaultTtl);
                object value;
                if (helper.TryGetFromCache(out value))
                {
                    invocation.ReturnValue = value;
                    return;
                }
                invocation.Proceed();
                value = invocation.ReturnValue;
                helper.AddToCache(value);
            }
            finally
            {
                _logger?.LogDebug("Exit interceptor for {0}.{1} ", invocation.TargetType, invocation.Method.Name);
            }
        }

        private static void ThrowIfNoTtlsAreProvided(IDictionary<string, TimeSpan> cacheTtLs, TimeSpan? defaultTtl)
        {
            if ((cacheTtLs == null || !cacheTtLs.Any()) && defaultTtl == null)
            {
                throw new ArgumentException($"One of \"{nameof(cacheTtLs)}\" or \"{nameof(defaultTtl)}\" must be provided.");
            }
        }

        private static void ThrowIfNoCacheProvider(ICache cacheProvider)
        {
            if (cacheProvider == null)
            {
                throw new ArgumentException($"\"{nameof(cacheProvider)}\" is mandatory.");
            }
        }


        private class CacheInterceptionHelper
        {
            private readonly ICache _cacheProvider;
            private readonly ILogger _logger;
            private readonly IDictionary<string, Func<object[], string>> _customCacheKeyGenerators;
            private readonly IDictionary<string, TimeSpan> _cacheTtLs;
            private readonly TimeSpan? _defaultTtl;
            private readonly IInvocation _invocation;
            private readonly CacheInterceptorConfigurationAttribute _configAttribute;
            private readonly bool _useCache;
            private readonly string _cacheKey;

            public CacheInterceptionHelper(IInvocation invocation, ICache cacheProvider, ILogger logger,
                IDictionary<string, Func<object[], string>> customCacheKeyGenerators,
                IDictionary<string, TimeSpan> cacheTtLs, TimeSpan? defaultTtl = null)
            {
                _invocation = invocation;
                _cacheProvider = cacheProvider;
                _logger = logger;
                _customCacheKeyGenerators = customCacheKeyGenerators;
                _cacheTtLs = cacheTtLs;
                _defaultTtl = defaultTtl;

                _configAttribute = GetAttribute(invocation);
                if (_configAttribute != null && _configAttribute.UseCache)
                {
                    _useCache = true;
                    _cacheKey = GetConfiguredCacheKey(invocation, _configAttribute) ??
                                CreateCacheKey(invocation, _configAttribute);


                    if (string.IsNullOrWhiteSpace(_cacheKey))
                        throw new Exception("Failed to obtain a cache key");
                }
                else
                {
                    _useCache = false;
                    _cacheKey = null;
                    _logger?.LogDebug("No caching for {0}.{1} ", invocation.TargetType, invocation.Method.Name);
                }
            }

            public bool TryGetFromCache(out object cached)
            {
                var cachedValue = _cacheProvider.Get(_cacheKey);
                var isInCache = cachedValue.HasValue;
                cached = isInCache ? cachedValue.Value : null;
                return isInCache;
            }

            public void AddToCache(object toCache)
            {
                //if there is no config attribute, then no cache is used, return immediately
                if (!_useCache)
                    return;

                //if the return is null it's only cached if explicitly indicated in the attribute CacheNullValues
                if (!_configAttribute.CacheNullValues && toCache == null)
                    return;

                //if the return is an empty collection it's only cached if explicitly indicated in the attribute CacheEmptyCollectionValues
                if (!_configAttribute.CacheEmptyCollectionValues && toCache is IEnumerable && !CollectionHasElements((IEnumerable)toCache))
                    return;

                var ttl = GetTtl();
                
                _cacheProvider.Add(_cacheKey, toCache, ttl);
            }

            private static CacheInterceptorConfigurationAttribute GetAttribute(IInvocation invocation)
            {
                //Get type CacheInterceptorConfigurationAttribute metadata.
                var configurationAttributeType = typeof(CacheInterceptorConfigurationAttribute);
                var configAttributes = invocation.MethodInvocationTarget.GetCustomAttributes(configurationAttributeType,
                    false);
                var enumeratedAttributes = configAttributes.ToArray();
                //if there are a config attribute defined and UseCache = true
                var configAttribute =
                    enumeratedAttributes.Select(a => a as CacheInterceptorConfigurationAttribute).SingleOrDefault();
                return configAttribute;
            }

            private string GetConfiguredCacheKey(IInvocation invocation,
                CacheInterceptorConfigurationAttribute configAttribute)
            {
                if (_customCacheKeyGenerators != null && !string.IsNullOrWhiteSpace(configAttribute.MethodKey) &&
                    _customCacheKeyGenerators.ContainsKey(configAttribute.MethodKey))
                {
                    var cacheKey = _customCacheKeyGenerators[configAttribute.MethodKey](invocation.Arguments);
                    _logger?.LogDebug($"Fetched configured cache key: \"{cacheKey}\"");
                    return cacheKey;
                }
                return null;
            }

            private string CreateCacheKey(IInvocation invocation, CacheInterceptorConfigurationAttribute configAttribute)
            {
                var argumentsToIgnore = configAttribute.ArgumentsToIgnoreOnKeyCreation ?? Array.Empty<string>();
                //fetch generic arguments and parameters
                var genericArguments = invocation.GenericArguments ?? Array.Empty<Type>();
                var parameters = invocation.MethodInvocationTarget.GetParameters();

                //prepare parameters string representation "type name: value"
                var parametersString = new List<string>();
                for (var i = 0; i < parameters.Count(); ++i)
                {
                    var parameterInfo = parameters[i];
                    if (argumentsToIgnore.Contains(parameterInfo.Name))
                    {
                        continue;
                    }
                    parametersString.Add(string.Format("{0} {1}:{2}", parameterInfo.ParameterType, parameterInfo.Name,
                        invocation.Arguments[i]));
                }

                //construct the cache key, "<generic arguments>method name(parameters)"
                var cacheKey = string.Format("<{0}>{1}({2})",
                    string.Join(",", genericArguments.Select(ga => ga.Name)),
                    invocation.MethodInvocationTarget.Name,
                    string.Join(",", parametersString)
                );

                _logger?.LogDebug($"Created cache key: \"{cacheKey}\"");
                return cacheKey;
            }

            //IEnumerable doesn't have a Count() or Any extension method, so this is needed in the Intercept method
            private static bool CollectionHasElements(IEnumerable collection)
            {
                return collection.Cast<object>().Any();
            }

            private TimeSpan GetTtl()
            {
                TimeSpan ttl;
                //get the ttl or throw error if it cannot be found
                if (_cacheTtLs.ContainsKey(_configAttribute.MethodKey))
                {
                    ttl = _cacheTtLs[_configAttribute.MethodKey];
                }
                else if (_defaultTtl != null)
                {
                    ttl = _defaultTtl.Value;
                    //using the defaultTTL should not be the norm, so log as debug when it happens
                    _logger?.LogDebug($"Using the default ttl for method: \"{_configAttribute.MethodKey}\"");
                }
                else
                {
                    throw new Exception(
                        $"Could not get a TTL for method \"{_configAttribute.MethodKey}\". Define it in the dicionary or provide a defaultTTL");
                }
                return ttl;
            }
        }
    }
}