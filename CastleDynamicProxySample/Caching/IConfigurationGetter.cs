using Castle.DynamicProxy;

namespace CodingMilitia.CastleDynamicProxySample.Caching
{
    public interface IConfigurationGetter
    {
        MethodCacheConfiguration Get(IInvocation invocation);
    }
}
