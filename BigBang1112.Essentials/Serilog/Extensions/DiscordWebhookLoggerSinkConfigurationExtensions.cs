using BigBang1112.Serilog.Sinks;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace BigBang1112.Serilog.Extensions;

public static class DiscordWebhookLoggerSinkConfigurationExtensions
{
    public static LoggerConfiguration DiscordWebhook(this LoggerSinkConfiguration loggerConfiguration, string webhookUrl, IFormatProvider? formatProvider = null, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error)
    {
        return loggerConfiguration.Sink(new DiscordWebhookSink(webhookUrl, formatProvider), restrictedToMinimumLevel);
    }
}
