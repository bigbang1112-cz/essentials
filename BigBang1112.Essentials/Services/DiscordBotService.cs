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

    public Dictionary<string, Type> Commands { get; }
    public Dictionary<Type, DiscordBotCommandAttribute> CommandAttributes { get; }
    public Dictionary<Type, Dictionary<string, PropertyInfo>> CommandOptions { get; }
    public Dictionary<PropertyInfo, DiscordBotCommandOptionAttribute> CommandOptionAttributes { get; }
    public Dictionary<PropertyInfo, MethodInfo> CommandOptionAutocompleteMethods { get; }

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

        Commands = new();
        CommandAttributes = new();
        CommandOptions = new();
        CommandOptionAttributes = new();
        CommandOptionAutocompleteMethods = new();
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        CreateCommandDefinitions();

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

    private void CreateCommandDefinitions()
    {
        foreach (var type in GetType().Assembly.GetTypes())
        {
            if (!Attribute.IsDefined(type, typeof(DiscordBotCommandAttribute)) || !type.IsSubclassOf(typeof(DiscordBotCommand)))
            {
                continue;
            }

            var commandType = type;

            var commandAtt = commandType.GetCustomAttribute<DiscordBotCommandAttribute>();

            if (commandAtt is null)
            {
                continue;
            }

            Commands[commandAtt.Name.ToLower()] = commandType;
            CommandAttributes[commandType] = commandAtt;
            CommandOptions[commandType] = CreateOptionDictionary(commandType);
        }
    }

    private Dictionary<string, PropertyInfo> CreateOptionDictionary(Type commandType)
    {
        var optionDict = new Dictionary<string, PropertyInfo>();

        foreach (var property in commandType.GetProperties())
        {
            var optionAtt = property.GetCustomAttribute<DiscordBotCommandOptionAttribute>();

            if (optionAtt is null)
            {
                continue;
            }

            optionDict[optionAtt.Name.ToLower()] = property;
            CommandOptionAttributes[property] = optionAtt;

            var autocompleteMethod = commandType.GetMethod($"Autocomplete{property.Name}Async");

            if (autocompleteMethod is not null)
            {
                CommandOptionAutocompleteMethods[property] = autocompleteMethod;
            }
        }

        return optionDict;
    }

    protected virtual async Task SlashCommandExecutedAsync(SocketSlashCommand slashCommand)
    {
        var stopwatch = Stopwatch.StartNew();

        using var scope = CreateCommand(slashCommand, out DiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        var message = await command.ExecuteAsync(slashCommand);

        await slashCommand.RespondAsync(message.Message ?? GetExecutedInMessage(stopwatch),
            message.Embeds,
            ephemeral: message.Ephemeral,
            components: message.Component);
    }

    protected virtual async Task SelectMenuExecutedAsync(SocketMessageComponent messageComponent)
    {
        var stopwatch = Stopwatch.StartNew();

        using var scope = CreateCommand(messageComponent, out DiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        var message = await command.SelectMenuAsync(messageComponent);

        if (message is null)
        {
            return;
        }

        if (message.AlwaysPostAsNewMessage)
        {
            await messageComponent.RespondAsync(message.Message ?? GetExecutedInMessage(stopwatch), message.Embeds,
                ephemeral: message.Ephemeral, components: message.Component);
        }
        else
        {
            await messageComponent.UpdateAsync(x =>
            {
                x.Content = message.Message ?? GetExecutedInMessage(stopwatch);
                x.Embeds = message.Embeds;

                if (message.Component is not null)
                {
                    x.Components = message.Component;
                }
            });
        }
    }

    protected virtual async Task AutocompleteExecutedAsync(SocketAutocompleteInteraction interaction)
    {
        var commandType = GetCommandType(interaction);

        using var scope = CreateCommand(commandType, out DiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        var option = interaction.Data.Current;

        if (!CommandOptions.TryGetValue(commandType, out var options))
        {
            return;
        }

        if (!options.TryGetValue(option.Name, out var prop))
        {
            return;
        }

        if (!CommandOptionAutocompleteMethods.TryGetValue(prop, out var autocompleteMethod))
        {
            return;
        }

        if (autocompleteMethod?.Invoke(command, new object[] { option.Value }) is not Task<IEnumerable<string>> autocompleteTask)
        {
            await interaction.RespondAsync(null);
            return;
        }

        await interaction.RespondAsync((await autocompleteTask).Select(x => new AutocompleteResult(x, x)));
    }

    protected virtual Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        return Task.CompletedTask;
    }

    private static string GetExecutedInMessage(Stopwatch stopwatch)
    {
        return $"Executed in {stopwatch.Elapsed.TotalSeconds:0.000}s.";
    }

    private IServiceScope? CreateCommand(SocketSlashCommand slashCommand, out DiscordBotCommand? command)
    {
        var type = GetCommandType(slashCommand);

        if (!CommandOptions.TryGetValue(type, out var options))
        {
            command = null;
            return null;
        }

        var scope = CreateCommand(type, out command);

        foreach (var option in slashCommand.Data.Options)
        {
            if (options.TryGetValue(option.Name, out var prop))
            {
                prop.SetValue(command, option.Value);
            }
        }

        return scope;
    }

    private IServiceScope? CreateCommand(SocketMessageComponent messageComponent, out DiscordBotCommand? command)
    {
        var split = messageComponent.Data.CustomId.Split('-');

        if (split.Length == 0)
        {
            command = null;
            return null;
        }

        var commandName = split[0].Replace('_', ' ');

        return CreateCommand(GetCommandType(commandName), out command);
    }

    private IServiceScope? CreateCommand(SocketAutocompleteInteraction interaction, out DiscordBotCommand? command)
    {
        return CreateCommand(GetCommandType(interaction), out command);
    }

    private IServiceScope? CreateCommand(Type commandType, out DiscordBotCommand? command)
    {
        var constructors = commandType.GetConstructors();

        if (constructors.Length > 1)
        {
            throw new Exception();
        }

        var constructor = constructors[0];

        var scope = _serviceProvider.CreateScope();

        var args = constructor.GetParameters()
            .Select(x =>
            {
                if (x.ParameterType.IsSubclassOf(typeof(DiscordBotService)))
                {
                    return this;
                }

                return scope.ServiceProvider.GetService(x.ParameterType);
            })
            .ToArray();

        command = (DiscordBotCommand)constructor.Invoke(args);

        return scope;
    }

    private Type GetCommandType(SocketSlashCommand slashCommand)
    {
        return GetCommandType(slashCommand.CommandName, slashCommand.Data.Options);
    }

    private Type GetCommandType(SocketAutocompleteInteraction interaction)
    {
        return GetCommandType(interaction.Data.CommandName, interaction.Data.Options);
    }

    private Type GetCommandType(string commandName, IEnumerable<AutocompleteOption> options)
    {
        var subCommands = options
            .Where(x => x.Type == ApplicationCommandOptionType.SubCommand || x.Type == ApplicationCommandOptionType.SubCommandGroup)
            .Select(x => x.Name);

        if (subCommands.Any())
        {
            commandName = $"{commandName} {string.Join(' ', subCommands)}";
        }

        return GetCommandType(commandName);
    }

    private Type GetCommandType(string commandName, IEnumerable<IApplicationCommandInteractionDataOption> options)
    {
        var subCommands = options
            .Where(x => x.Type == ApplicationCommandOptionType.SubCommand || x.Type == ApplicationCommandOptionType.SubCommandGroup)
            .Select(x => x.Name);

        if (subCommands.Any())
        {
            commandName = $"{commandName} {string.Join(' ', subCommands)}";
        }

        return GetCommandType(commandName);
    }

    private Type GetCommandType(string commandName)
    {
        if (Commands.TryGetValue(commandName, out Type? commandType))
        {
            return commandType;
        }

        throw new Exception();
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
        var guild = Client.GetGuild(ulong.Parse(_config["DiscordGuild"]));

        foreach (var (commandName, commandType) in Commands)
        {
            if (!CommandAttributes.TryGetValue(commandType, out DiscordBotCommandAttribute? commandAttribute))
            {
                continue;
            }

            var slashCommand = new SlashCommandBuilder
            {
                Name = commandName,
                Description = commandAttribute.Description
            };

            if (!CommandOptions.TryGetValue(commandType, out var options))
            {
                continue;
            }

            foreach (var (optionName, property) in options)
            {
                if (!CommandOptionAttributes.TryGetValue(property, out var optAtt))
                {
                    continue;
                }

                slashCommand.AddOption(optionName, optAtt.Type, optAtt.Description, optAtt.IsRequired,
                    optAtt.IsDefault, isAutocomplete: CommandOptionAutocompleteMethods.ContainsKey(property),
                    optAtt.MinValue, optAtt.MaxValue);
            }

            var command = slashCommand.Build();

#if DEBUG
            await guild.CreateApplicationCommandAsync(command);
#else
            await Client.CreateGlobalApplicationCommandAsync(command);
#endif
        }
    }
}
