using Serilog.Core;
using Serilog.Events;

namespace BigBang1112.Serilog.Enrichers;

public class ServiceNameEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        _ = logEvent ?? throw new ArgumentNullException(nameof(logEvent));
        _ = propertyFactory ?? throw new ArgumentNullException(nameof(propertyFactory));

        if (!logEvent.Properties.TryGetValue("SourceContext", out var propertyValue))
        {
            return;
        }

        if (propertyValue is not ScalarValue scalarValue || scalarValue.Value is not string value)
        {
            return;
        }

        var serviceName = value[(value.LastIndexOf('.') + 1)..];

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ServiceName", serviceName));
    }
}
