using System;
using CodingMilitia.CastleDynamicProxySample.Caching;

namespace CodingMilitia.CachingSampleBenchmarksApplication.Service
{
    public class StuffService : IStuffService
    {
        [CacheInterceptorConfiguration(MethodKey = "GetGenericStuffKey", UseCache = true)]
        public TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn) => stuffToReturn;

        [CacheInterceptorConfiguration(MethodKey = "GetGenericStuffGenerateKeyKey", UseCache = true)]
        public TOut GetGenericStuffGenerateKey<TOut, TIn>(TIn stuffId, TOut stuffToReturn) => stuffToReturn;
    }
}