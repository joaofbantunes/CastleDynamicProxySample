# CastleDynamicProxySample
A sample of some things we can do with Castle DynamicProxy

The CastleDynamicProxySample project contains samples for two scenarios where the DynamicProxy can ease your life: caching and checking an operation performance.

## CacheInterceptor
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

We can see there is a difference in the order of magnitute between the proxy approach and the decorator approach. This is not due (mostly) with using the DynamicProxy, but because this approach requires some under the hood "magic" to work (intercept only the correct methods, creating the cache keys dynamically and so on) that the decorator doesn't because it's coded for each specific situation.

## TimingInterceptor
The TimingInterceptor shows a way to check the time an operation takes to complete, like CacheInterceptor, avoiding the implementation of a decorator and the repetitive timeing logic.

The TimingSampleBenchmarksApplication project benchmarks the TimingInterceptor and puts it against a decorator implementation for the same interface in order to check for the performance differences.

```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Windows
Processor=?, ProcessorCount=8
Frequency=2740588 ticks, Resolution=364.8852 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003133

Type=TimingBenchmark  Mode=Throughput  

```
       Method |      Median |    StdDev |
------------- |------------ |---------- |
 DynamicProxy | 963.4282 ns | 6.8242 ns |
    Decorator | 866.9434 ns | 4.6971 ns |
    
This is a better example to check the performance difference between the proxy and the decorator approachs than the CacheInterceptor, because to do the timing of the operation we don't need so much "magic" going on in the interceptor.

## TODO
* Support async methods

### Note
This code was not thoroughly tested so if you find it useful and want to use some or all of it, be sure to write some unit tests for it.
