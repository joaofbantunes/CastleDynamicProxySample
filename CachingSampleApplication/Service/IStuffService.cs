using System;

namespace CodingMilitia.CachingSampleApplication.Service
{
    public interface IStuffService
    {
        string GetStuff(string stuffId);

        string[] GetSomeMoreStuff();

        TOut GetGenericStuff<TOut, TIn>(TIn stuffId, TOut stuffToReturn);

        string GetStuffWithoutCache(string stuffId);
    }
}