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
Frequency=2740595 ticks, Resolution=364.8843 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003133

Type=CacheBenchmark  Mode=Throughput  

```
                       Method |         Median |        StdDev |
----------------------------- |--------------- |-------------- |
       ProxyWithGeneratedKeys | 41,730.7652 ns | 1,116.8795 ns |
      ProxyWithConfiguredKeys | 26,851.7763 ns | 1,477.9911 ns |
  ProxyWithGeneratedKeysAsync | 48,725.3231 ns | 1,881.9533 ns |
 ProxyWithConfiguredKeysAsync | 32,384.6814 ns |   492.3214 ns |
                    Decorator |    407.1695 ns |     7.6731 ns |
               DecoratorAsync |    503.9454 ns |    29.7041 ns |

We can see there is a difference in the order of magnitute between the proxy approach and the decorator approach. This is not due (mostly) with using the DynamicProxy, but because this approach requires some under the hood "magic" to work (intercept only the correct methods, creating the cache keys dynamically and so on) that the decorator doesn't because it's coded for each specific situation.
It's also visible that, to support async methods, there's a bit more overhead, as a little more reflection is used.

## TimingInterceptor
The TimingInterceptor shows a way to check the time an operation takes to complete, like CacheInterceptor, avoiding the implementation of a decorator and the repetitive timeing logic.

The TimingSampleBenchmarksApplication project benchmarks the TimingInterceptor and puts it against a decorator implementation for the same interface in order to check for the performance differences.

```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Windows
Processor=?, ProcessorCount=8
Frequency=2740595 ticks, Resolution=364.8843 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003133

Type=TimingBenchmark  Mode=Throughput  

```
                      Method |        Median |      StdDev |
---------------------------- |-------------- |------------ |
                DynamicProxy |   220.8362 ns |  12.0907 ns |
           DynamicProxyAsync |   320.8456 ns |   7.9605 ns |
 DynamicProxyWithResultAsync | 2,050.8260 ns | 113.5469 ns |
                   Decorator |    51.6744 ns |   0.8392 ns |
              DecoratorAsync |   116.1886 ns |   1.5978 ns |
    DecoratorWithResultAsync |   134.1244 ns |   1.7090 ns |
    
This is a better example to check the performance difference between the proxy and the decorator approaches than the CacheInterceptor, because to do the timing of the operation we don't need so much "magic" going on in the interceptor.
We can see that when using async methods, mainly the ones that return Task<T> instead of just Task, there is a more noticeable performance penalty due to the use of reflection needed to implment this scenario.

## TODO
* Better test async method support.

### Note
This code was not thoroughly tested so if you find it useful and want to use some or all of it, be sure to write some unit tests for it.
