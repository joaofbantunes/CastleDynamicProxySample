using System;

namespace CodingMilitia.CachingSampleBenchmarksApplication.Service
{
    public interface IStuffService
    {
        TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn);
        TOut GetGenericStuffGenerateKey<TOut, TIn>(TIn stuffId, TOut stuffToReturn);
    }
}