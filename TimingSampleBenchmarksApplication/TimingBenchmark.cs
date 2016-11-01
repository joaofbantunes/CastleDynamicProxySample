using BenchmarkDotNet.Attributes;
using CodingMilitia.TimingSampleBenchmarksApplication.Service;
using CodingMilitia.CastleDynamicProxySample;
using CodingMilitia.CastleDynamicProxySample.Timing;
using System.Threading.Tasks;

namespace CodingMilitia.TimingSampleBenchmarksApplication
{
    public class TimingBenchmark
    {
        private readonly IStuffService _proxiedService;
        private readonly IStuffService _decoratedService;

        public TimingBenchmark()
        {
            var proxyGenerator = new ProxyGenerator();
            var service = new StuffService();
            var timingInterceptor = new TimingInterceptor(null);
            _proxiedService = proxyGenerator.CreateInterfaceProxyWithTarget<IStuffService>(service, timingInterceptor);
            _decoratedService = new StuffServiceTimingDecorator(null, service);
        }

        [Benchmark]
        public void DynamicProxy()
        {
            _proxiedService.DoTimeConsumingStuff();
        }
        [Benchmark]
        public void DynamicProxyAsync()
        {
            Task.WaitAll(_proxiedService.DoTimeConsumingStuffAsync());
            
        }
        [Benchmark]
        public void DynamicProxyWithResultAsync()
        {
            Task.WaitAll(_proxiedService.DoTimeConsumingStuffAndGetAsync());
        }

        [Benchmark]
        public void Decorator()
        {
            _decoratedService.DoTimeConsumingStuff();
        }
        [Benchmark]
        public void DecoratorAsync()
        {
            Task.WaitAll(_decoratedService.DoTimeConsumingStuffAsync());

        }
        [Benchmark]
        public void DecoratorWithResultAsync()
        {
            Task.WaitAll(_decoratedService.DoTimeConsumingStuffAndGetAsync());
        }
    }
}
