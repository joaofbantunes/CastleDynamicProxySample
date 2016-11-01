using CodingMilitia.CastleDynamicProxySample.Caching;
using CodingMilitia.CastleDynamicProxySample.Caching.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

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

        [CacheInterceptorConfiguration(MethodId = "GetGenericStuffAsyncKey", UseCache = true)]
        public Task<TOut> GetGenericStuffAsync<TOut, TIn>(TIn stuffId, TOut stuffToReturn) => Task.FromResult(stuffToReturn);

        public string GetStuffWithoutCache(string stuffId) => "Returning " + stuffId + " without being cached";

        [CacheInterceptorConfiguration(MethodId = "GetGenericStuffGenerateKeyAsyncKey", UseCache = true)]
        public async Task<int> GetStatusCodeAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://codingmilitia.com");
                return (int)response.StatusCode;
            }
        }
    }
}