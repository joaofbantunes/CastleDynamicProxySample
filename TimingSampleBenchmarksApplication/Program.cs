
using BenchmarkDotNet.Running;

namespace CodingMilitia.TimingSampleBenchmarksApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TimingBenchmark>();
        }
    }
}
