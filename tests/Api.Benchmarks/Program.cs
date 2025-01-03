using BenchmarkDotNet.Running;

namespace Api.Benchmarks;

public sealed partial class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
