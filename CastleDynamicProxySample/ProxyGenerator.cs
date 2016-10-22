using Castle.DynamicProxy;

namespace CodingMilitia.CastleDynamicProxySample
{
    public class ProxyGenerator
    {
        //If this was instantiated every time it's needed, then multiple generated types would be created and the memory would grow indefinitely.
        //Being static makes use of the same generated types, stabilizing the memory consumption (and improving performance, as the generation of new types is slow and costly).
        private static readonly Castle.DynamicProxy.ProxyGenerator CastleProxyGenerator;

        static ProxyGenerator()
        {
            CastleProxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
        }

        public TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class
        {
            return CastleProxyGenerator.CreateClassProxy<TClass>(interceptors);
        }
        public TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors) where TClass : class
        {
            return CastleProxyGenerator.CreateClassProxyWithTarget(target, interceptors);
        }

        public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params IInterceptor[] interceptors) where TInterface : class
        {
            return CastleProxyGenerator.CreateInterfaceProxyWithTarget(target, interceptors);
        }
    }
}