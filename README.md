# CastleDynamicProxySample
A sample of some things we can do with Castle DynamicProxy

The CastleDynamicProxySample project contains samples for two scenarios where the DynamicProxy can ease your life: caching and checking an operation performance.

CacheInterceptor shows a way that uses the DynamicProxy to avoid implementing a decorator and the repetitive caching logic.
Added the CachingSampleApplication project just to show some ways to use the implemented CacheInterceptor.

The CachingSampleBenchmarksApplication project benchmarks the CacheInterceptor and puts it against a decorator implementation for the same interface in order to check for the performance differences.

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
                  Method |         Median |      StdDev |
------------------------ |--------------- |------------ |
  ProxyWithGeneratedKeys | 40,180.3769 ns | 293.9969 ns |
 ProxyWithConfiguredKeys | 25,621.3372 ns |  96.2125 ns |
               Decorator |    399.2209 ns |   3.9400 ns |
442 ns |
ark_Decorator
    428.5731 ns |    13.1603 ns |
