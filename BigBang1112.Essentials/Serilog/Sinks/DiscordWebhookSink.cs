using System.Timers;
using Discord;
using Discord.Webhook;
using Serilog.Core;
using Serilog.Events;
using System;

namespace BigBang1112.Serilog.Sinks;

public class DiscordWebhookSink : ILogEventSink
{
    private readonly DiscordWebhookClient webhook;
    private readonly IFormatProvider? formatProvider;
    private readonly System.Timers.Timer timer;

    private readonly Queue<LogEvent> queue;

    public DiscordWebhookSink(string webhookUrl, IFormatProvider? formatProvider = null)
    {
        webhook = new DiscordWebhookClient(webhookUrl);
        queue = new Queue<LogEvent>();

        this.formatProvider = formatProvider;
        
        timer = new System.Timers.Timer(5000)
        {
            AutoReset = true
        };
        
        timer.Elapsed += SendEmbeds;
        timer.Start();
    }

    private async void SendEmbeds(object? source, ElapsedEventArgs e)
    {
        var embedCount = 0;

        var list = new List<Embed>();

        while (queue.Count > 0)
        {
            var logEvent = queue.Dequeue();
            var message = logEvent.RenderMessage(formatProvider)
                .Replace("\"", "**");

            var color = logEvent.Level switch
            {
                LogEventLevel.Verbose => new Color(0, 0, 0),
                LogEventLevel.Debug => new Color(0, 0, 0),
                LogEventLevel.Information => new Color(0, 0, 0),
                LogEventLevel.Warning => new Color(255, 255, 0),
                LogEventLevel.Error => new Color(255, 0, 0),
                LogEventLevel.Fatal => new Color(255, 0, 0),
                _ => new Color(0, 0, 0)
            };

            if (logEvent.Exception is not null)
            {
                message = logEvent.Exception.Message;

                var exceptionStr = logEvent.Exception.ToString();

                if (exceptionStr.Length > 1000)
                {
                    exceptionStr = string.Concat(exceptionStr.AsSpan(0, 1000), "...");
                }

                message += $" ```\n{exceptionStr}\n```";
            }

            var embedBuilder = new EmbedBuilder
            {
                Description = message,
                Timestamp = logEvent.Timestamp,
                Color = color
            };

            if (logEvent.Properties.TryGetValue("ServiceName", out LogEventPropertyValue? propValue))
            {
                if (propValue is not ScalarValue scalarValue || scalarValue.Value is not string value)
                {
                    return;
                }

                embedBuilder.Title = value;
            }

            list.Add(embedBuilder.Build());

            embedCount++;

            if (embedCount % 10 == 0)
            {
                await webhook.SendMessageAsync(embeds: list);
                list.Clear();
            }
        }

        if (list.Count > 0)
        {
            await webhook.SendMessageAsync(embeds: list);
        }
    }

    public void Emit(LogEvent logEvent)
    {
        queue.Enqueue(logEvent);
    }
}
