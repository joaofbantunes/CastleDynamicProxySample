using Microsoft.Extensions.Logging;
using System;

namespace CodingMilitia.TimingSampleBenchmarksApplication.Service
{
    public class StuffServiceTimingDecorator : IStuffService
    {
        private readonly IStuffService _continuation;
        private readonly ILogger<StuffServiceTimingDecorator> _logger;

        public StuffServiceTimingDecorator(ILoggerFactory loggerFactory, IStuffService continuation)
        {
            _logger = loggerFactory?.CreateLogger<StuffServiceTimingDecorator>();
            _continuation = continuation;
        }

        public void DoTimeConsumingStuff()
        {
            //DateTime doesn't have the best precision, but it's enough for this sample
            DateTime requestStartTime = DateTime.Now;
            _logger?.LogDebug(string.Format("{0}.{1}() started at - {2}",
                _continuation.GetType().DeclaringType,
                nameof(DoTimeConsumingStuff),
                requestStartTime));
            try
            {
                _continuation.DoTimeConsumingStuff();
            }
            finally
            {
                DateTime requestEndTime = DateTime.Now;
                _logger?.LogDebug(string.Format("{0}.{1}() ended at - {2} - took around {3}ms to complete",
                    _continuation.GetType().DeclaringType,
                    nameof(DoTimeConsumingStuff),
                    requestEndTime,
                    (requestEndTime - requestStartTime).TotalMilliseconds));
            }
        }
    }
}
