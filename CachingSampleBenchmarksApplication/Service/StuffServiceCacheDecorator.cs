using CodingMilitia.CastleDynamicProxySample.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingMilitia.CachingSampleBenchmarksApplication.Service
{
    public class StuffServiceCacheDecorator : IStuffService
    {
        private static readonly TimeSpan _ttl = TimeSpan.FromMinutes(2);
        private readonly ICache _cache;
        private readonly IStuffService _continuation;
        
        public StuffServiceCacheDecorator(ICache cache, IStuffService continuation)
        {
            _cache = cache;
            _continuation = continuation;
        }

        public TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn)
        {
            string cacheKey = $"GetGenericStuff(\"{stuffId}\",\"{stuffToReturn}\")";
            ICachedObject<TOut> cachedValue = _cache.Get<TOut>(cacheKey);
            if (cachedValue.HasObject)
            {
                return cachedValue.Object;
            }
            var value = _continuation.GetGenericStuff(stuffId, stuffToReturn);
            _cache.Add(cacheKey,stuffToReturn,_ttl);
            return value;            
        }

        public TOut GetGenericStuffGenerateKey<TOut, TIn>(TIn stuffId, TOut stuffToReturn)
        {
            throw new NotImplementedException();
        }
    }
}
