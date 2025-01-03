using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using Api.Benchmarks;

[assembly: ExcludeFromCodeCoverage]
[assembly: Config(typeof(MyConfig))]
