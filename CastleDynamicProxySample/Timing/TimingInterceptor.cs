using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;

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
            //DateTime doesn't have the best precision, but it's enough for this sample
            DateTime requestStartTime = DateTime.Now;
            _logger?.LogDebug(string.Format("{0}.{1}() started at - {2}", 
                invocation.MethodInvocationTarget.DeclaringType, 
                invocation.MethodInvocationTarget.Name, 
                requestStartTime));
            try
            {
                invocation.Proceed();
            }
            finally
            {
                DateTime requestEndTime = DateTime.Now;
                _logger?.LogDebug(string.Format("{0}.{1}() ended at - {2} - took around {3}ms to complete", 
                    invocation.MethodInvocationTarget.DeclaringType,
                    invocation.MethodInvocationTarget.Name,
                    requestEndTime, 
                    (requestEndTime - requestStartTime).TotalMilliseconds));
            }
        }
    }
}
