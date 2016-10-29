using System;
using CodingMilitia.CastleDynamicProxySample.Caching;
using CodingMilitia.CastleDynamicProxySample.Caching.Configuration;

namespace CodingMilitia.CachingSampleBenchmarksApplication.Service
{
    public class StuffService : IStuffService
    {
        [CacheInterceptorConfiguration(MethodId = "GetGenericStuffKey", UseCache = true)]
        public TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn) => stuffToReturn;

        [CacheInterceptorConfiguration(MethodId = "GetGenericStuffGenerateKeyKey", UseCache = true)]
        public TOut GetGenericStuffGenerateKey<TOut, TIn>(TIn stuffId, TOut stuffToReturn) => stuffToReturn;
    }
}