using System;
using System.Threading.Tasks;

namespace CodingMilitia.CachingSampleApplication.Service
{
    public interface IStuffService
    {
        string GetStuff(string stuffId);

        string[] GetSomeMoreStuff();

        TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn);

        Task<TOut> GetGenericStuffAsync<TOut, TIn>(TIn stuffId, TOut stuffToReturn);

        string GetStuffWithoutCache(string stuffId);
        Task<int> GetStatusCodeAsync();
    }
}