using System;

namespace CodingMilitia.CastleDynamicProxySample.Caching
{
    public interface ICache : IDisposable
    {
        void Add<T>(string key, T value, TimeSpan timeToLive);

        ICachedObject<T> Get<T>(string key);

        void Remove(string key);
    }

    public interface ICachedObject<out T>
    {
        bool HasObject { get; }
        T Object { get; }
    }
}