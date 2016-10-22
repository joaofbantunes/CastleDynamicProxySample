﻿using System;
using System.Collections.Generic;
using CodingMilitia.CachingSampleApplication.Mock;
using CodingMilitia.CachingSampleApplication.Service;
using CodingMilitia.CastleDynamicProxySample;
using CodingMilitia.CastleDynamicProxySample.Caching;
using Microsoft.Extensions.Logging;


namespace CodingMilitia.CachingSampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IDictionary<string, TimeSpan> cacheTTLs = new Dictionary<string, TimeSpan>
            {
                {"GetStuffKey", new TimeSpan(0, 5, 0)},
                {"GetGenericStuffKey", new TimeSpan(0, 3, 0)}
            };

            IDictionary<string, Func<object[], string>> cacheKeyGenerators = new Dictionary<string, Func<object[], string>>
            {
                {"GetSomeMoreStuffKey", GenerateRandomKey}
            };

            var loggerFactory = new LoggerFactory().AddConsole(LogLevel.Trace);
            var cache = new MockCache(loggerFactory);
            var cacheInterceptor = new CacheInterceptor(cache, loggerFactory, cacheKeyGenerators, cacheTTLs,
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
            Console.WriteLine(nameof(proxiedService.GetStuffWithoutCache));
            Console.WriteLine(proxiedService.GetStuffWithoutCache("Identifier"));
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