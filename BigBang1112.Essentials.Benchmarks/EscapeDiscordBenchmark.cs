using BenchmarkDotNet.Attributes;
using BigBang1112.Extensions;

namespace BigBang1112.Benchmarks;

[MemoryDiagnoser]
public class EscapeDiscordBenchmark
{
    private const string ExampleString = "**This** *is* _a test_ ||string||";
    private const string ExampleStringNoFormatting = "This is a test string";

    [Benchmark]
    public string EscapeDiscord()
    {
        return ExampleString.EscapeDiscord();
    }

    [Benchmark]
    public string EscapeDiscord_Old()
    {
        return ExampleString.Replace("_", "\\_").Replace("*", "\\*").Replace("|", "\\|");
    }

    [Benchmark]
    public string EscapeDiscord_NoFormatting()
    {
        return ExampleStringNoFormatting.EscapeDiscord();
    }

    [Benchmark]
    public string EscapeDiscord_Old_NoFormatting()
    {
        return ExampleStringNoFormatting.Replace("_", "\\_").Replace("*", "\\*").Replace("|", "\\|");
    }
}
