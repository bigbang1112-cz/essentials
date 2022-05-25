using BenchmarkDotNet.Running;
using BigBang1112.Benchmarks;

var result = BenchmarkRunner.Run(typeof(EscapeDiscordBenchmark));