using Castle.DynamicProxy;
using CodingMilitia.CastleDynamicProxySample.Helpers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using System;
using System.Collections.Generic;

namespace CodingMilitia.CastleDynamicProxySample.Timing
{
    public class TimingInterceptor : IInterceptor
    {
        private readonly ILogger<TimingInterceptor> _logger;
        private readonly IDictionary<Type, MethodInfo> _genericMethodCache;

        public TimingInterceptor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<TimingInterceptor>();
            _genericMethodCache = new Dictionary<Type, MethodInfo>();
        }

        void IInterceptor.Intercept(IInvocation invocation)
        {
            _logger?.LogDebug(string.Format("Entered {0}.{1}()",
                invocation.MethodInvocationTarget.DeclaringType,
                invocation.MethodInvocationTarget.Name));

            if (AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                ProceedAsync(invocation);
            }
            else
            {
                ProceedSync(invocation);
            }

        }

        private void ProceedAsync(IInvocation invocation)
        {
            var watch = Stopwatch.StartNew();
            invocation.Proceed();
            if (invocation.Method.ReturnType == typeof(Task))
            {
                
                invocation.ReturnValue = AsyncHelper.AwaitTaskWithFinally((Task)invocation.ReturnValue, () => LogExiting(invocation, watch));
            }
            else //Task<TResult>
            {
                Action<object> finallyLog = (value) => LogExiting(invocation, watch);
                invocation.ReturnValue = GetGenericMethod(invocation).Invoke(null, new object[] { invocation.ReturnValue, finallyLog });
            }
        }

        private void ProceedSync(IInvocation invocation)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                LogExiting(invocation, watch);
            }
        }

        private void LogExiting(IInvocation invocation, Stopwatch watch)
        {
            watch.Stop();
            _logger?.LogDebug("Exiting {0}.{1}() - took around {2}ms to complete",
                    invocation.MethodInvocationTarget.DeclaringType,
                    invocation.MethodInvocationTarget.Name,
                    watch.ElapsedMilliseconds);
        }

        private MethodInfo GetGenericMethod(IInvocation invocation)
        {
            var genericReturnType = invocation.Method.ReturnType.GenericTypeArguments[0];
            MethodInfo genericMethod;
            if (!_genericMethodCache.TryGetValue(genericReturnType, out genericMethod))
            {
                genericMethod = typeof(AsyncHelper)
                            .GetMethod(nameof(AsyncHelper.AwaitTaskWithFinallyAndGetResult), BindingFlags.Public | BindingFlags.Static)
                            .MakeGenericMethod(genericReturnType);
                _genericMethodCache[genericReturnType] = genericMethod;
            }
            return genericMethod;
        }
    }
}
