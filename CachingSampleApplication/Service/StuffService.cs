using CodingMilitia.CastleDynamicProxySample.Caching;
using CodingMilitia.CastleDynamicProxySample.Caching.Configuration;

namespace CodingMilitia.CachingSampleApplication.Service
{
    public class StuffService : IStuffService
    {
        [CacheInterceptorConfiguration(MethodId = "GetStuffKey", UseCache = true)]
        public string GetStuff(string stuffId) => "Returning " + stuffId;

        [CacheInterceptorConfiguration(MethodId = "GetSomeMoreStuffKey", UseCache = true)]
        public string[] GetSomeMoreStuff() => new string[] {"One Stuff", "Two Stuff"};

        [CacheInterceptorConfiguration(MethodId = "GetGenericStuffKey", UseCache = true)]
        public TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn) => stuffToReturn;

        public string GetStuffWithoutCache(string stuffId) => "Returning " + stuffId + " without being cached";
    }
}