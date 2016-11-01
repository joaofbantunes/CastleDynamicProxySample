using System;
using System.Threading.Tasks;
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

        [CacheInterceptorConfiguration(MethodId = "GetGenericStuffAsyncKey", UseCache = true)]
        public Task<TOut> GetGenericStuffAsync<TOut, TIn>(TIn stuffId, TOut stuffToReturn) => Task.FromResult(stuffToReturn);

        [CacheInterceptorConfiguration(MethodId = "GetGenericStuffGenerateKeyAsyncKey", UseCache = true)]
        public Task<TOut> GetGenericStuffGenerateKeyAsync<TOut, TIn>(TIn stuffId, TOut stuffToReturn) => Task.FromResult(stuffToReturn);
    }
}