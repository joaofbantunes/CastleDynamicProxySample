using System;
using System.Collections.Generic;
using CodingMilitia.CachingSampleApplication.Mock;
using CodingMilitia.CachingSampleApplication.Service;
using CodingMilitia.CastleDynamicProxySample;
using CodingMilitia.CastleDynamicProxySample.Caching;
using Microsoft.Extensions.Logging;
using CodingMilitia.CastleDynamicProxySample.Caching.CacheKeyCreation.Composite;
using CodingMilitia.CastleDynamicProxySample.Caching.CacheKeyCreation.Configuration;
using CodingMilitia.CastleDynamicProxySample.Caching.CacheKeyCreation.Reflection;
using CodingMilitia.CastleDynamicProxySample.Caching.Configuration;
using System.Threading.Tasks;

namespace CodingMilitia.CachingSampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IDictionary<string, Func<object[], string>> cacheKeyGenerators = new Dictionary<string, Func<object[], string>>
            {
                {"GetSomeMoreStuffKey", GenerateRandomKey}
            };

            var loggerFactory = new LoggerFactory().AddConsole(LogLevel.Trace);
            var cache = new MockCache(loggerFactory);
            var cacheInterceptor = new CacheInterceptor(cache, loggerFactory,
                new CompositeCacheKeyCreationStrategy(loggerFactory,
                    new ConfigurationBasedCacheKeyCreationStrategy(cacheKeyGenerators, loggerFactory),
                    new ReflectionBasedCacheKeyCreationStrategy(null, loggerFactory)),
                new AttributeBasedConfigurationGetter(),
                new TimeSpan(0, 2, 0));
            var proxyGenerator = new ProxyGenerator();
            var service = new StuffService();
            var proxiedService = proxyGenerator.CreateInterfaceProxyWithTarget<IStuffService>(service, cacheInterceptor);

            WriteSeparator();
            Console.WriteLine(nameof(proxiedService.GetStuff));
            Console.WriteLine(proxiedService.GetStuff("Identifier"));
            WriteSeparator();
            Console.WriteLine(nameof(proxiedService.GetSomeMoreStuff));
            Console.WriteLine(proxiedService.GetSomeMoreStuff());
            WriteSeparator();
            Console.WriteLine(nameof(proxiedService.GetGenericStuff));
            Console.WriteLine(proxiedService.GetGenericStuff<string, int>(1, "Return This"));
            WriteSeparator();
            Console.WriteLine(nameof(proxiedService.GetGenericStuffAsync));
            Console.WriteLine(proxiedService.GetGenericStuffAsync<string, int>(1, "Return This Async").Result);
            WriteSeparator();
            Console.WriteLine(nameof(proxiedService.GetStuffWithoutCache));
            Console.WriteLine(proxiedService.GetStuffWithoutCache("Identifier"));
            WriteSeparator();
            Console.WriteLine(nameof(proxiedService.GetStatusCodeAsync));
            Console.WriteLine(proxiedService.GetStatusCodeAsync().Result);
            WriteSeparator();
        }

        private static void WriteSeparator()
        {
            Console.WriteLine("---------------------------------------------------------------");
        }

        private static string GenerateRandomKey(params object[] methodArguments)
        {
            return "ANotSoRandomKey...ButKinda";
        }
    }
}