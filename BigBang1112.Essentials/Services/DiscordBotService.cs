using BigBang1112.Attributes;
using BigBang1112.Attributes.DiscordBot;
using BigBang1112.Models;
using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BigBang1112.Services;

public abstract class DiscordBotService : IHostedService
{
    public const int OptionLimit = 25;

    private readonly IServiceProvider _serviceProvider;

    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    protected DiscordSocketClient Client { get; }
    protected IDictionary<string, Type> Commands { get; }

    public DiscordBotService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _logger = serviceProvider.GetRequiredService<ILogger<DiscordBotService>>();
        _config = serviceProvider.GetRequiredService<IConfiguration>();

        Client = new DiscordSocketClient();
        Client.Ready += ReadyAsync;
        Client.Log += LogAsync;
        Client.MessageReceived += MessageReceivedAsync;
        Client.InteractionCreated += InteractionCreatedAsync;
        Client.SlashCommandExecuted += SlashCommandExecutedAsync;
        Client.AutocompleteExecuted += AutocompleteExecutedAsync;
        Client.SelectMenuExecuted += SelectMenuExecutedAsync;

        Commands = GetType().Assembly
            .GetTypes()
            .Where(x => Attribute.IsDefined(x, typeof(DiscordBotCommandAttribute)) && x.GetInterface(nameof(IDiscordBotCommand)) is not null)
            .ToDictionary(x => x.GetCustomAttribute<DiscordBotCommandAttribute>()!.Name.ToLower());
    }

    protected virtual async Task SlashCommandExecutedAsync(SocketSlashCommand slashCommand)
    {
        using var scope = CreateCommand(slashCommand.CommandName, out IDiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        await command.ExecuteAsync(slashCommand);
    }

    protected virtual async Task SelectMenuExecutedAsync(SocketMessageComponent messageComponent)
    {
        var split = messageComponent.Data.CustomId.Split('-');
        
        if (split.Length == 0)
        {
            return;
        }

        var commandName = split[0];

        using var scope = CreateCommand(commandName, out IDiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        await command.SelectMenuAsync(messageComponent, messageComponent.Data.Values);
    }

    protected virtual Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        return Task.CompletedTask;
    }

    protected virtual async Task AutocompleteExecutedAsync(SocketAutocompleteInteraction interaction)
    {
        using var scope = CreateCommand(interaction.Data.CommandName, out IDiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        await command.AutocompleteAsync(interaction, interaction.Data.Current);
    }

    private IServiceScope? CreateCommand(string commandName, out IDiscordBotCommand? command)
    {
        if (!Commands.TryGetValue(commandName, out Type? commandType))
        {
            command = null;
            return null;
        }

        var constructors = commandType.GetConstructors();

        if (constructors.Length > 1)
        {
            throw new Exception();
        }

        var constructor = constructors[0];

        var scope = _serviceProvider.CreateScope();

        var args = constructor.GetParameters()
            .Select(x => scope.ServiceProvider.GetService(x.ParameterType))
            .ToArray();

        command = (IDiscordBotCommand)constructor.Invoke(args);

        return scope;
    }

    protected virtual Task MessageReceivedAsync(SocketMessage arg)
    {
        return Task.CompletedTask;
    }

    private Task LogAsync(LogMessage arg)
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
                _logger.LogWarning(arg.Exception, "{message}", arg.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("{message}", arg.Message);
                break;
        }

        return Task.CompletedTask;
    }

    protected virtual async Task ReadyAsync()
    {
        foreach (var (commandName, commandType) in Commands)
        {
            var slashCommand = new SlashCommandBuilder();

            var commandAttribute = commandType.GetCustomAttribute<DiscordBotCommandAttribute>();

            if (commandAttribute is null)
            {
                continue;
            }

            slashCommand.Name = commandName;
            slashCommand.Description = commandAttribute.Description;

            using var scope = CreateCommand(commandName, out IDiscordBotCommand? command);

            if (command is null)
            {
                continue;
            }

            foreach (var option in command.YieldOptions())
            {
                slashCommand.AddOption(option);
            }

            var finalCommand = slashCommand.Build();

#if DEBUG
            var guild = Client.GetGuild(ulong.Parse(_config["DiscordGuild"]));
            await guild.CreateApplicationCommandAsync(finalCommand);
#else
            await Client.CreateGlobalApplicationCommandAsync(finalCommand);
#endif
        }
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        var secret = _config[GetType().GetCustomAttribute<SecretAppsettingsPathAttribute>()?.Path ?? throw new Exception()];

        if (string.IsNullOrWhiteSpace(secret))
        {
            return;
        }

        await Client.LoginAsync(TokenType.Bot, secret);
        await Client.StartAsync();
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        await Client.StopAsync();
    }
}
