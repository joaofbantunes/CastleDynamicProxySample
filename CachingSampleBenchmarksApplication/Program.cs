using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using CodingMilitia.CachingSampleBenchmarksApplication;

namespace CachingSampleBenchmarksApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CacheBenchmark>(
                ManualConfig.Create(DefaultConfig.Instance).With(MemoryDiagnoser.Default)
            );
        }
    }
}
