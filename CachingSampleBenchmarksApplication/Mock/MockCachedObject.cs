using CodingMilitia.CastleDynamicProxySample.Caching;

namespace CodingMilitia.CachingSampleBenchmarksApplication.Mock
{
    public class MockCachedObject<T> : ICachedObject<T>
    {
        public bool HasObject { get; private set; }
        public T Object { get; private set;}

        public MockCachedObject(bool hasObject, T @object)
        {
            HasObject = hasObject;
            Object = @object;
        }
    }
}