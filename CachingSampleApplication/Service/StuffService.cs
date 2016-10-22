using CodingMilitia.CastleDynamicProxySample.Caching;

namespace CodingMilitia.CachingSampleApplication.Service
{
    public class StuffService : IStuffService
    {
        [CacheInterceptorConfiguration(MethodKey = "GetStuffKey", UseCache = true)]
        public string GetStuff(string stuffId) => "Returning " + stuffId;

        [CacheInterceptorConfiguration(MethodKey = "GetSomeMoreStuffKey", UseCache = true)]
        public string[] GetSomeMoreStuff() => new string[] {"One Stuff", "Two Stuff"};

        [CacheInterceptorConfiguration(MethodKey = "GetGenericStuffKey", UseCache = true)]
        public TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn) => stuffToReturn;

        public string GetStuffWithoutCache(string stuffId) => "Returning " + stuffId + " without being cached";
    }
}