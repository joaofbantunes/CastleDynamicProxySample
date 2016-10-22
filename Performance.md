On commit https://github.com/joaofbantunes/CastleDynamicProxySample/commit/7e477794225accff2ab92c03557079bf95f75097 was able to make a good performance improvement by simplifying the ICache interface to not use generics, avoiding the use of reflection (`MakeGenericMethod`) on the CacheInterceptor.

Improved from:

```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Windows
Processor=?, ProcessorCount=8
Frequency=2740588 ticks, Resolution=364.8852 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003133

Type=CacheBenchmark  Mode=Throughput  

```
                  Method |         Median |        StdDev |
------------------------ |--------------- |-------------- |
  ProxyWithGeneratedKeys | 77,754.2210 ns | 2,576.9225 ns |
 ProxyWithConfiguredKeys | 64,585.3930 ns | 7,029.3754 ns |
               Decorator |    433.0167 ns |    26.0543 ns |

To:

```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Windows
Processor=?, ProcessorCount=8
Frequency=2740588 ticks, Resolution=364.8852 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003133

Type=CacheBenchmark  Mode=Throughput  

```
                  Method |         Median |        StdDev |
------------------------ |--------------- |-------------- |
  ProxyWithGeneratedKeys | 43,466.8158 ns | 5,293.5027 ns |
 ProxyWithConfiguredKeys | 29,020.3328 ns |   449.7977 ns |
               Decorator |    410.5160 ns |    11.7768 ns |
