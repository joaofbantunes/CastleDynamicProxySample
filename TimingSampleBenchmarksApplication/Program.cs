
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

namespace CodingMilitia.TimingSampleBenchmarksApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TimingBenchmark>(
                ManualConfig.Create(DefaultConfig.Instance).With(MemoryDiagnoser.Default)
            );
        }
    }
}
