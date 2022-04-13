using BigBang1112.Serilog.Enrichers;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BigBang1112.Extensions;

public static class HostBuilderEssentialsExtensions
{
    public static IHostBuilder UseEssentials(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
            config.Enrich.With<ServiceNameEnricher>();
            config.WriteTo.File("./logs/log-.txt", rollingInterval: RollingInterval.Day);
            config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {ServiceName}] {Message:lj}{NewLine}{Exception}");
        });

        return hostBuilder;
    }
}
