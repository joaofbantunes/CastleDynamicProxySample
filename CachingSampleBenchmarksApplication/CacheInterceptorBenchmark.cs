using BenchmarkDotNet.Attributes;
using CodingMilitia.CachingSampleBenchmarksApplication.Mock;
using CodingMilitia.CachingSampleBenchmarksApplication.Service;
using CodingMilitia.CastleDynamicProxySample;
using CodingMilitia.CastleDynamicProxySample.Caching;
using CodingMilitia.CastleDynamicProxySample.Caching.CacheKeyCreation.Composite;
using CodingMilitia.CastleDynamicProxySample.Caching.CacheKeyCreation.Configuration;
using CodingMilitia.CastleDynamicProxySample.Caching.CacheKeyCreation.Reflection;
using CodingMilitia.CastleDynamicProxySample.Caching.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingMilitia.CachingSampleBenchmarksApplication
{
    public class CacheBenchmark
    {
        private readonly IStuffService _proxiedService;
        private readonly IStuffService _decoratedService;

        public CacheBenchmark()
        {
            IDictionary<string, Func<object[], string>> cacheKeyGenerators = new Dictionary<string, Func<object[], string>>
            {
                {"GetGenericStuffKey", ConfiguredKeyGenerator},
                {"GetGenericStuffAsyncKey", ConfiguredKeyGenerator}
            };

            var cache = new MockCache(null);
            var cacheInterceptor = new CacheInterceptor(cache, null,
                new CompositeCacheKeyCreationStrategy(null,
                    new ConfigurationBasedCacheKeyCreationStrategy(cacheKeyGenerators, null),
                    new ReflectionBasedCacheKeyCreationStrategy(null, null)),
                new AttributeBasedConfigurationGetter(null),
                new TimeSpan(0, 2, 0));
            var proxyGenerator = new ProxyGenerator();
            var service = new StuffService();
            _proxiedService = proxyGenerator.CreateInterfaceProxyWithTarget<IStuffService>(service, cacheInterceptor);
            _decoratedService = new StuffServiceCacheDecorator(cache, null, service);
        }

        [Benchmark]
        public void ProxyWithGeneratedKeys()
        {
            _proxiedService.GetGenericStuffGenerateKey("Id", "Return");
        }

        [Benchmark]
        public void ProxyWithConfiguredKeys()
        {
            _proxiedService.GetGenericStuff("Id", "Return");
        }

        [Benchmark]
        public void ProxyWithGeneratedKeysAsync()
        {
            Task.WaitAll(_proxiedService.GetGenericStuffGenerateKeyAsync("Id", "Return"));
        }

        [Benchmark]
        public void ProxyWithConfiguredKeysAsync()
        {
            Task.WaitAll(_proxiedService.GetGenericStuffAsync("Id", "Return"));
        }

        [Benchmark]
        public void Decorator()
        {
            _decoratedService.GetGenericStuff("Id", "Return");
        }

        [Benchmark]
        public void DecoratorAsync()
        {
            Task.WaitAll(_decoratedService.GetGenericStuffAsync("Id", "Return"));
        }

        private static string ConfiguredKeyGenerator(params object[] methodArguments)
        {
            return $"GetGenericStuff(\"{methodArguments[0]}\",\"{methodArguments[1]}\")";
        }
    }
}
