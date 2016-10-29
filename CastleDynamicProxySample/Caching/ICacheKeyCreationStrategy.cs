using Castle.DynamicProxy;

namespace CodingMilitia.CastleDynamicProxySample.Caching
{
    public interface ICacheKeyCreationStrategy
    {
        string Create(string methodId, IInvocation invocation);
    }
}
