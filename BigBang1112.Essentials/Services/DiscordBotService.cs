using BigBang1112.Attributes;
using BigBang1112.Attributes.DiscordBot;
using BigBang1112.Models;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
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
    protected SlashCommandProperties[]? SlashCommands { get; private set; }

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
        using var scope = CreateCommand(GetFullCommandName(slashCommand), out IDiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        var message = await command.ExecuteAsync(slashCommand);

        await slashCommand.RespondAsync(message.Message ?? GetExecutedInMessage(stopwatch),
            embeds: message.Embeds,
            ephemeral: message.Ephemeral,
            components: message.Component);
    }

    private static string GetExecutedInMessage(Stopwatch stopwatch)
    {
        return $"Executed in {stopwatch.Elapsed.TotalSeconds:0.000}s. Further time caused by Discord API.";
    }

    private static string GetFullCommandName(SocketSlashCommand slashCommand)
    {
        return GetFullCommandName(slashCommand.Data.Name, slashCommand.Data.Options);
    }

    private static string GetFullCommandName(string commandName, IReadOnlyCollection<SocketSlashCommandDataOption> options)
    {
        var subCommands = options
            .Where(x => x.Type == ApplicationCommandOptionType.SubCommand)
            .Select(x => x.Name);

        return subCommands.Any() ? $"{commandName} {string.Join(' ', subCommands)}" : commandName;
    }

    protected virtual async Task SelectMenuExecutedAsync(SocketMessageComponent messageComponent)
    {
        var split = messageComponent.Data.CustomId.Split('-');
        
        if (split.Length == 0)
        {
            return;
        }

        var commandName = split[0];

        using var scope = CreateCommand(commandName.Replace('_', ' '), out IDiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        var message = await command.SelectMenuAsync(messageComponent, messageComponent.Data.Values);

        await messageComponent.UpdateAsync(x =>
        {
            x.Content = message.Message ?? GetExecutedInMessage(stopwatch);
            x.Embeds = message.Embeds;
        });
    }

    protected virtual Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        return Task.CompletedTask;
    }

    protected virtual async Task AutocompleteExecutedAsync(SocketAutocompleteInteraction interaction)
    {
        var subCommands = interaction.Data.Options
            .Where(x => x.Type == ApplicationCommandOptionType.SubCommand)
            .Select(x => x.Name);

        var fullCommandName = subCommands.Any()
            ? $"{interaction.Data.CommandName} {string.Join(' ', subCommands)}"
            : interaction.Data.CommandName;

        using var scope = CreateCommand(fullCommandName, out IDiscordBotCommand? command);

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
        var commandDefinitions = new Dictionary<string, SlashCommandBuilder>();

        foreach (var (commandName, commandType) in Commands)
        {
            var commandNameAndSubCommands = commandName.Split(' ');

            var mainCmd = commandNameAndSubCommands[0];

            var commandAttribute = commandType.GetCustomAttribute<DiscordBotCommandAttribute>();

            if (commandAttribute is null)
            {
                continue;
            }

            if (!commandDefinitions.TryGetValue(mainCmd, out SlashCommandBuilder? slashCommand))
            {
                slashCommand = new SlashCommandBuilder();
            }

            using var scope = CreateCommand(commandName, out IDiscordBotCommand? command);

            if (command is null)
            {
                continue;
            }

            if (commandNameAndSubCommands.Length == 0)
            {
                continue;
            }
            else if (commandNameAndSubCommands.Length == 1)
            {
                var cmd = commandNameAndSubCommands[0];

                slashCommand.Name = cmd;
                slashCommand.Description = commandAttribute.Description;

                foreach (var option in command.YieldOptions())
                {
                    slashCommand.AddOption(option);
                }
            }
            else if (commandNameAndSubCommands.Length > 1)
            {
                slashCommand.Name ??= commandNameAndSubCommands[1 - 1];
                slashCommand.Description ??= "No description.";

                var cmd = commandNameAndSubCommands[1];

                slashCommand.AddOption(cmd, ApplicationCommandOptionType.SubCommand, commandAttribute.Description,
                    options: command.YieldOptions().ToList());
            }

            commandDefinitions[mainCmd] = slashCommand;
        }

        var guild = Client.GetGuild(ulong.Parse(_config["DiscordGuild"]));

        SlashCommands = commandDefinitions.Values.Select(x => x.Build()).ToArray();

        //await guild.BulkOverwriteApplicationCommandAsync(SlashCommands);
        
        foreach (var command in SlashCommands)
        {
#if DEBUG
            await guild.CreateApplicationCommandAsync(command);
#else
            await Client.CreateGlobalApplicationCommandAsync(command);
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
