using System;

namespace CodingMilitia.CastleDynamicProxySample.Caching
{
    public interface ICache
    {
        void Add(string key, object value, TimeSpan timeToLive);

        CachedValue Get(string key);
    }

    public class CachedValue
    {
        public bool HasValue { get; private set; }
        public object Value { get; private set; }

        public CachedValue(bool hasValue, object value)
        {
            HasValue = hasValue;
            Value = value;
        }
    }
}