using System;
using System.Threading.Tasks;

namespace CodingMilitia.CachingSampleBenchmarksApplication.Service
{
    public interface IStuffService
    {
        TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn);
        TOut GetGenericStuffGenerateKey<TOut, TIn>(TIn stuffId, TOut stuffToReturn);
        Task<TOut> GetGenericStuffAsync<TOut, TIn>(TIn stuffId, TOut stuffToReturn);
        Task<TOut> GetGenericStuffGenerateKeyAsync<TOut, TIn>(TIn stuffId, TOut stuffToReturn);

    }
}