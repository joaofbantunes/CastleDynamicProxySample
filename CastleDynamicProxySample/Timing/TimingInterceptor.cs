using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace CodingMilitia.CastleDynamicProxySample.Timing
{
    public class TimingInterceptor : IInterceptor
    {
        private readonly ILogger<TimingInterceptor> _logger;
        public TimingInterceptor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<TimingInterceptor>();
        }

        void IInterceptor.Intercept(IInvocation invocation)
        {
            _logger?.LogDebug(string.Format("Entered {0}.{1}()", 
                invocation.MethodInvocationTarget.DeclaringType, 
                invocation.MethodInvocationTarget.Name));
            var watch = Stopwatch.StartNew();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                watch.Stop();

                _logger?.LogDebug(string.Format("Exiting {0}.{1}() - took around {2}ms to complete",
              invocation.MethodInvocationTarget.DeclaringType,
              invocation.MethodInvocationTarget.Name,
              watch.ElapsedMilliseconds));
            }
        }
    }
}
