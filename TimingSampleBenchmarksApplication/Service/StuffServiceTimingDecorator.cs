using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

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

            _logger?.LogDebug(string.Format("Entered {0}.{1}()",
                _continuation.GetType().Name,
                nameof(DoTimeConsumingStuff)));
            var watch = Stopwatch.StartNew();
            try
            {
                _continuation.DoTimeConsumingStuff();
            }
            finally
            {
                _logger?.LogDebug(string.Format("Exiting {0}.{1}() - took around {2}ms to complete",
                    _continuation.GetType().Name,
                    nameof(DoTimeConsumingStuff),
                    watch.ElapsedMilliseconds));
            }
        }
    }
}
