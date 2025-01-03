using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Validators;

namespace Api.Benchmarks;

public sealed class MyConfig : ManualConfig
{
    public MyConfig()
    {
        AddJob(Job.Dry);
        AddLogger(ConsoleLogger.Default);
        AddValidator(JitOptimizationsValidator.DontFailOnError);
    }
}
