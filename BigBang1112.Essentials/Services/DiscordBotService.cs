using BigBang1112.Attributes;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BigBang1112.Services;

public abstract class DiscordBotService : IHostedService
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
    private readonly DiscordSocketClient client;

    protected DiscordSocketClient Client => client;

    public DiscordBotService(IConfiguration config, ILogger logger)
    {
        _config = config;
        _logger = logger;

        client = new DiscordSocketClient();
        client.Ready += Ready;
        client.Log += Log;
    }

    private Task Log(LogMessage arg)
    {
        switch (arg.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical(arg.Exception, "{message}", arg.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError(arg.Exception, "{message}", arg.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning("{message}", arg.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("{message}", arg.Message);
                break;
        }

        return Task.CompletedTask;
    }

    protected virtual Task Ready()
    {
        return Task.CompletedTask;
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        var secret = _config[GetType().GetCustomAttribute<SecretAppsettingsPathAttribute>()?.Path ?? throw new Exception()];

        if (string.IsNullOrWhiteSpace(secret))
        {
            return;
        }

        await client.LoginAsync(TokenType.Bot, secret);
        await client.StartAsync();
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.StopAsync();
    }
}
