using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

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

        public Task<int> DoTimeConsumingStuffAndGetAsync()
        {
            _logger?.LogDebug(string.Format("Entered {0}.{1}()",
               _continuation.GetType().Name,
               nameof(DoTimeConsumingStuff)));
            var watch = Stopwatch.StartNew();
            try
            {
                return _continuation.DoTimeConsumingStuffAndGetAsync();
            }
            finally
            {
                _logger?.LogDebug(string.Format("Exiting {0}.{1}() - took around {2}ms to complete",
                    _continuation.GetType().Name,
                    nameof(DoTimeConsumingStuff),
                    watch.ElapsedMilliseconds));
            }
        }

        public Task DoTimeConsumingStuffAsync()
        {
            _logger?.LogDebug(string.Format("Entered {0}.{1}()",
               _continuation.GetType().Name,
               nameof(DoTimeConsumingStuff)));
            var watch = Stopwatch.StartNew();
            try
            {
                return _continuation.DoTimeConsumingStuffAsync();
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
