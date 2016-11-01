using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using System.Reflection;

namespace CodingMilitia.CastleDynamicProxySample.Caching.Configuration
{
    public class AttributeBasedConfigurationGetter : IConfigurationGetter
    {
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
                result.Ttl = !string.IsNullOrWhiteSpace(config.Ttl) ? new TimeSpan?(TimeSpan.Parse(config.Ttl)) : null;
            }
            return result;
        }
    }
}
