using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace CodingMilitia.CastleDynamicProxySample.Caching.CacheKeyCreation.Reflection
{
    public class ReflectionBasedCacheKeyCreationStrategy : ICacheKeyCreationStrategy
    {
        private readonly ILogger<ReflectionBasedCacheKeyCreationStrategy> _logger;
        private readonly Func<string, IEnumerable<string>> _argumentsToIgnoreGetter;

        public ReflectionBasedCacheKeyCreationStrategy(Func<string,IEnumerable<string>> argumentsToIgnoreGetter, ILoggerFactory loggerFactory)
        {
            _argumentsToIgnoreGetter = argumentsToIgnoreGetter;
            _logger = loggerFactory?.CreateLogger<ReflectionBasedCacheKeyCreationStrategy>();

        }

        public string Create(string methodId, IInvocation invocation)
        {
            var methodArgumentsToIgnore = _argumentsToIgnoreGetter?.Invoke(methodId) ?? Enumerable.Empty<string>();
            //fetch generic arguments and parameters
            var genericArguments = invocation.GenericArguments ?? Array.Empty<Type>();
            var parameters = invocation.MethodInvocationTarget.GetParameters();

            //prepare parameters string representation "type name: value"
            var parametersString = new List<string>();
            for (var i = 0; i < parameters.Count(); ++i)
            {
                var parameterInfo = parameters[i];
                if (methodArgumentsToIgnore.Contains(parameterInfo.Name))
                {
                    continue;
                }
                parametersString.Add(string.Format("{0} {1}:{2}", parameterInfo.ParameterType, parameterInfo.Name,
                    invocation.Arguments[i]));
            }

            //construct the cache key, "<generic arguments>full type name.method name(parameters)"
            var cacheKey = string.Format("<{0}>{1}.{2}({3})",
                string.Join(",", genericArguments.Select(ga => ga.Name)),
                invocation.TargetType.FullName,
                invocation.MethodInvocationTarget.Name,
                string.Join(",", parametersString)
            );

            _logger?.LogDebug($"Created cache key: \"{cacheKey}\"");
            return cacheKey;
        }
    }
}
