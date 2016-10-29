using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using System.Reflection;

namespace CodingMilitia.CastleDynamicProxySample.Caching.Configuration
{
    public class AttributeBasedConfigurationGetter : IConfigurationGetter
    {
        private readonly IDictionary<string, TimeSpan> _ttls;

        public AttributeBasedConfigurationGetter(IDictionary<string, TimeSpan> ttls)
        {
            _ttls = ttls ?? new Dictionary<string, TimeSpan>();
        }

        public MethodCacheConfiguration Get(IInvocation invocation)
        {
            var config = invocation.MethodInvocationTarget
                .GetCustomAttribute(typeof(CacheInterceptorConfigurationAttribute), false) as CacheInterceptorConfigurationAttribute;

            var result = new MethodCacheConfiguration();

            if (config == null)
            {
                result.UseCache = false;
            }
            else
            {
                result.UseCache = config.UseCache;
                result.MethodId = config.MethodId;
                result.CacheNullValues = config.CacheNullValues;
                result.CacheEmptyCollectionValues = config.CacheEmptyCollectionValues;
                TimeSpan ttl;
                if (_ttls.TryGetValue(config.MethodId, out ttl))
                {
                    result.Ttl = ttl;
                }
            }
            return result;
        }
    }
}
