using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace BigBang1112;

public class EssentialsOptions
{
    public string Title { get; init; } = "BigBang1112";
    public Assembly Assembly { get; init; } = default!;
    public IConfiguration Config { get; init; } = default!;

    public static EssentialsOptions FromDelegate(Action<EssentialsOptions> options)
    {
        var essentialsOptions = new EssentialsOptions();
        options(essentialsOptions);
        return essentialsOptions;
    }
}
