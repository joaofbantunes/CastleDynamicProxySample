# CastleDynamicProxySample
A sample of some things we can do with Castle DynamicProxy

The CastleDynamicProxySample project contains samples for two scenarios where the DynamicProxy can ease your life: caching and checking an operation performance.

## CacheInterceptor
CacheInterceptor shows a way that uses the DynamicProxy to avoid implementing a decorator and the repetitive caching logic.
Added the CachingSampleApplication project just to show some ways to use the implemented CacheInterceptor.

The CachingSampleBenchmarksApplication project benchmarks the CacheInterceptor and puts it against a decorator implementation for the same interface in order to check for the performance differences.

``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7 CPU 930 2.80GHz (Nehalem), ProcessorCount=8
Frequency=2740576 Hz, Resolution=364.8868 ns, Timer=TSC
.NET Core SDK=2.0.0
  [Host]     : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT


```
 |                       Method |        Mean |      Error |     StdDev |  Gen 0 | Allocated |
 |----------------------------- |------------:|-----------:|-----------:|-------:|----------:|
 |       ProxyWithGeneratedKeys | 35,170.5 ns | 349.044 ns | 291.468 ns | 0.9155 |    3953 B |
 |      ProxyWithConfiguredKeys | 22,436.0 ns | 250.722 ns | 234.525 ns | 0.4883 |    2121 B |
 |  ProxyWithGeneratedKeysAsync | 41,317.0 ns | 600.522 ns | 561.729 ns | 1.0376 |    4451 B |
 | ProxyWithConfiguredKeysAsync | 26,356.5 ns | 203.918 ns | 190.745 ns | 0.6104 |    2609 B |
 |                    Decorator |    426.5 ns |   2.484 ns |   2.324 ns | 0.0281 |     120 B |
 |               DecoratorAsync |    515.2 ns |   2.149 ns |   1.905 ns | 0.0525 |     224 B |

We can see there is a difference in the order of magnitute between the proxy approach and the decorator approach. This is not due (mostly) with using the DynamicProxy, but because this approach requires some under the hood "magic" to work (intercept only the correct methods, creating the cache keys dynamically and so on) that the decorator doesn't because it's coded for each specific situation.

It's also visible that, to support async methods, there's a bit more overhead, as a little more reflection is used.

## TimingInterceptor
The TimingInterceptor shows a way to check the time an operation takes to complete, like CacheInterceptor, avoiding the implementation of a decorator and the repetitive timeing logic.

The TimingSampleBenchmarksApplication project benchmarks the TimingInterceptor and puts it against a decorator implementation for the same interface in order to check for the performance differences.

``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7 CPU 930 2.80GHz (Nehalem), ProcessorCount=8
Frequency=2740576 Hz, Resolution=364.8868 ns, Timer=TSC
.NET Core SDK=2.0.0
  [Host]     : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT


```
 |                      Method |        Mean |     Error |    StdDev |  Gen 0 | Allocated |
 |---------------------------- |------------:|----------:|----------:|-------:|----------:|
 |                DynamicProxy |   173.03 ns | 2.0208 ns | 1.8902 ns | 0.0343 |     144 B |
 |           DynamicProxyAsync |   299.46 ns | 1.1482 ns | 1.0741 ns | 0.0663 |     280 B |
 | DynamicProxyWithResultAsync | 1,704.27 ns | 9.2421 ns | 8.1929 ns | 0.1125 |     480 B |
 |                   Decorator |    46.43 ns | 0.1403 ns | 0.1244 ns | 0.0095 |      40 B |
 |              DecoratorAsync |   103.72 ns | 0.8985 ns | 0.7965 ns | 0.0170 |      72 B |
 |    DecoratorWithResultAsync |   119.75 ns | 0.9498 ns | 0.8885 ns | 0.0343 |     144 B |

This is a better example to check the performance difference between the proxy and the decorator approaches than the CacheInterceptor, because to do the timing of the operation we don't need so much "magic" going on in the interceptor.

We can see that when using async methods, mainly the ones that return `Task<T>` instead of just `Task`, there is a more noticeable performance penalty due to the use of reflection, needed to implement this scenario.

### Note
This code was not thoroughly tested so if you find it useful and want to use some or all of it, be sure to write some unit tests for it.
