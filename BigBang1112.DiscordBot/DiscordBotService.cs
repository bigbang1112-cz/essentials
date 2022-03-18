﻿using BigBang1112.Attributes;
using BigBang1112.Data;
using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace BigBang1112.DiscordBot;

public abstract class DiscordBotService : IHostedService
{
    private readonly DiscordBotAttribute? attribute;

    private readonly IServiceProvider _serviceProvider;

    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    protected DiscordSocketClient Client { get; }

    public Dictionary<string, Type> Commands { get; }
    public Dictionary<Type, IEnumerable<Attribute>> CommandAttributes { get; }
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
        Client.GuildAvailable += GuildAvailableAsync;

        Commands = new();
        CommandAttributes = new();
        CommandOptions = new();
        CommandOptionAttributes = new();
        CommandOptionAutocompleteMethods = new();

        attribute = GetType().GetCustomAttribute<DiscordBotAttribute>();
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        CreateCommandDefinitions(typeof(DiscordBotService).Assembly.GetTypes());
        CreateCommandDefinitions(GetType().Assembly.GetTypes());

        var secret = _config[GetType().GetCustomAttribute<SecretAppsettingsPathAttribute>()?.Path ?? throw new Exception()];

        if (string.IsNullOrWhiteSpace(secret))
        {
            return;
        }

        await CreateDiscordBotInDatabase(cancellationToken);

        await Client.LoginAsync(TokenType.Bot, secret);
        await Client.StartAsync();
    }

    public Guid? GetGuid()
    {
        return attribute?.Guid;
    }

    private async Task CreateDiscordBotInDatabase(CancellationToken cancellationToken)
    {
        if (attribute is null)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        var discordBotRepo = scope.ServiceProvider.GetRequiredService<IDiscordBotRepo>();

        await discordBotRepo.AddOrUpdateDiscordBotAsync(attribute, cancellationToken);

        await discordBotRepo.SaveAsync(cancellationToken);
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        await Client.LogoutAsync();
    }

    private void CreateCommandDefinitions(Type[] types)
    {
        foreach (var type in types)
        {
            if (!Attribute.IsDefined(type, typeof(DiscordBotCommandAttribute)) || !type.IsSubclassOf(typeof(DiscordBotCommand)))
            {
                continue;
            }

            var commandType = type;

            var commandAtts = commandType.GetCustomAttributes();

            var commandName = commandAtts.OfType<DiscordBotCommandAttribute>()
                .First().Name.ToLower();

            Commands[commandName] = commandType;
            CommandAttributes[commandType] = commandAtts;
            CommandOptions[commandType] = CreateOptionDictionary(commandType);

            CreateSubCommands(commandType, commandName);
        }
    }

    private void CreateSubCommands(Type commandType, string commandName)
    {
        var nestedTypes = commandType.GetNestedTypes();

        foreach (var nestedType in nestedTypes.Where(x => x.IsSubclassOf(typeof(DiscordBotCommand))))
        {
            var atts = nestedType.GetCustomAttributes();

            var subCommandName = atts.OfType<DiscordBotSubCommandAttribute>()
                .FirstOrDefault()?.Name.ToLower();

            if (subCommandName is null)
            {
                continue;
            }

            var fullCommand = $"{commandName} {subCommandName}";

            CommandAttributes[nestedType] = atts;
            CommandOptions[nestedType] = CreateOptionDictionary(nestedType);
            Commands[fullCommand] = nestedType;

            CreateSubCommands(nestedType, fullCommand);
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

    protected virtual async Task GuildAvailableAsync(SocketGuild guild)
    {
        if (attribute is null)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        var discordBotRepo = scope.ServiceProvider.GetRequiredService<IDiscordBotRepo>();

        _ = await discordBotRepo.GetOrAddJoinedDiscordGuildAsync(attribute.Guid, guild);

        await discordBotRepo.SaveAsync();
    }

    protected virtual async Task SlashCommandExecutedAsync(SocketSlashCommand slashCommand)
    {
        var stopwatch = Stopwatch.StartNew();

        using var scope = CreateCommand(slashCommand, out DiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        var deferer = new Deferer(slashCommand);

        DiscordBotMessage message;

        try
        {
            message = await command.ExecuteAsync(slashCommand, deferer);
        }
        catch (Exception ex)
        {
            if (deferer.IsDeferred)
            {
                await slashCommand.FollowupAsync("Error:", embed: new EmbedBuilder().WithDescription(ex.ToString()).WithColor(255, 0, 0).Build());
            }
            else
            {
                await slashCommand.RespondAsync("Error:", embed: new EmbedBuilder().WithDescription(ex.ToString()).WithColor(255, 0, 0).Build());
            }

            return;
        }

        var discordBotRepo = scope!.ServiceProvider.GetRequiredService<IDiscordBotRepo>();

        var ephemeral = await SetVisibilityOfExecutionAsync(slashCommand, message, discordBotRepo);

        if (deferer.IsDeferred)
        {
            if (message.Attachment is null)
            {
                await slashCommand.FollowupAsync(message.Message ?? GetExecutedInMessage(stopwatch),
                    message.Embeds,
                    ephemeral: ephemeral,
                    components: message.Component);
            }
            else
            {
                await slashCommand.FollowupWithFileAsync(message.Attachment.Value,
                    message.Message ?? GetExecutedInMessage(stopwatch),
                    message.Embeds,
                    ephemeral: ephemeral,
                    components: message.Component);
            }
        }
        else
        {
            if (message.Attachment is null)
            {
                await slashCommand.RespondAsync(message.Message ?? GetExecutedInMessage(stopwatch),
                    message.Embeds,
                    ephemeral: ephemeral,
                    components: message.Component);
            }
            else
            {
                await slashCommand.RespondWithFileAsync(message.Attachment.Value,
                    message.Message ?? GetExecutedInMessage(stopwatch),
                    message.Embeds,
                    ephemeral: ephemeral,
                    components: message.Component);
            }
        }
    }

    private async Task<bool> SetVisibilityOfExecutionAsync(SocketSlashCommand slashCommand, DiscordBotMessage message, IDiscordBotRepo discordBotRepo)
    {
        if (slashCommand.Channel is not SocketTextChannel textChannel || attribute is null)
        {
            return true;
        }

        var joinedGuild = await discordBotRepo.GetJoinedDiscordGuildAsync(attribute.Guid, textChannel);

        if (joinedGuild is null)
        {
            return true;
        }

        var visibility = await discordBotRepo.GetDiscordBotCommandVisibilityAsync(joinedGuild, textChannel.Id);

        if (visibility is null)
        {
            if (joinedGuild.CommandVisibility)
            {
                return message.Ephemeral;
            }

            return true;
        }

        if (visibility.Visibility || visibility.JoinedGuild.CommandVisibility)
        {
            return message.Ephemeral;
        }

        return true;
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
        return $"Executed in {stopwatch.Elapsed.TotalSeconds:0.000}s";
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

        var realOptions = RecurseToRealOptions(slashCommand.Data.Options);

        foreach (var option in realOptions)
        {
            if (options.TryGetValue(option.Name, out var prop))
            {
                prop.SetValue(command, option.Value);
            }
        }

        return scope;
    }

    private IEnumerable<SocketSlashCommandDataOption> RecurseToRealOptions(IReadOnlyCollection<SocketSlashCommandDataOption> options)
    {
        foreach (var option in options)
        {
            if (option.Type == ApplicationCommandOptionType.SubCommand || option.Type == ApplicationCommandOptionType.SubCommandGroup)
            {
                foreach (var option2 in RecurseToRealOptions(option.Options))
                {
                    yield return option2;
                }
            }
            else
            {
                yield return option;
            }
        }
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
                if (x.ParameterType == typeof(DiscordBotService) || x.ParameterType.IsSubclassOf(typeof(DiscordBotService)))
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
        var subCommands = RecurseSubCommands(options);

        if (subCommands.Any())
        {
            commandName = $"{commandName} {string.Join(' ', subCommands)}";
        }

        return GetCommandType(commandName);

        static IEnumerable<string> RecurseSubCommands(IEnumerable<IApplicationCommandInteractionDataOption> options)
        {
            foreach (var option in options)
            {
                if (option.Type == ApplicationCommandOptionType.SubCommand || option.Type == ApplicationCommandOptionType.SubCommandGroup)
                {
                    yield return option.Name;

                    foreach (var innerOption in RecurseSubCommands(option.Options))
                    {
                        yield return innerOption;
                    }
                }
            }
        }
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

        if (guild is null)
        {
            return;
        }

        var commandBuilders = CreateSlashCommands();
        var commands = BuildSlashCommands(commandBuilders);

/*#if DEBUG
        await guild.BulkOverwriteApplicationCommandAsync(commands.ToArray());
#else
        await Client.BulkOverwriteApplicationCommandAsync(commands.ToArray());
#endif

        return;*/

        foreach (var command in commands)
        {
#if DEBUG
            await guild.CreateApplicationCommandAsync(command);
#else
            await Client.CreateGlobalApplicationCommandAsync(command);
#endif
        }
    }

    private static IEnumerable<SlashCommandProperties> BuildSlashCommands(IEnumerable<SlashCommandBuilder> commandBuilders)
    {
        foreach (var commandBuilder in commandBuilders)
        {
            yield return commandBuilder.Build();
        }
    }

    private IEnumerable<SlashCommandBuilder> CreateSlashCommands()
    {
        foreach (var (commandName, commandType) in Commands)
        {
            if (commandName.IndexOf(' ') > -1)
            {
                continue;
            }

            if (!CommandAttributes.TryGetValue(commandType, out IEnumerable<Attribute>? commandAttributes))
            {
                continue;
            }

            var commandAttribute = commandAttributes.OfType<DiscordBotCommandAttribute>().First();

            var slashCommand = new SlashCommandBuilder
            {
                Name = commandName,
                Description = commandAttribute.Description
            };

            if (!CommandOptions.TryGetValue(commandType, out var options))
            {
                continue;
            }

            var mainCommandOptions = CreateOptions(options).ToArray();

            if (mainCommandOptions.Length > 0)
            {
                slashCommand.AddOptions(mainCommandOptions);
            }

            var subCommandOptions = CreateSlashSubCommands(commandType, subCommandType: null, subCommandAttribute: null).ToArray();

            if (subCommandOptions.Length > 0)
            {
                slashCommand.AddOptions(subCommandOptions);
            }

            yield return slashCommand;
        }
    }

    private IEnumerable<SlashCommandOptionBuilder> CreateSlashSubCommands(Type commandType, Type? subCommandType, DiscordBotSubCommandAttribute? subCommandAttribute)
    {
        var nestedTypes = commandType.GetNestedTypes();

        var subCommandTypes = nestedTypes.Where(x => x.IsSubclassOf(typeof(DiscordBotCommand))
            && Attribute.IsDefined(x, typeof(DiscordBotSubCommandAttribute)))
            .ToList();

        if (subCommandType is not null && subCommandAttribute is not null)
        {
            if (subCommandTypes.Count > 0)
            {
                yield return new SlashCommandOptionBuilder
                {
                    Name = subCommandAttribute.Name,
                    Type = ApplicationCommandOptionType.SubCommandGroup,
                    Description = subCommandAttribute.Description,
                    Options = CreateSlashSubCommands(subCommandType, null, null).ToList()
                };

                yield break;
            }
            else if (subCommandTypes.Count == 0)
            {
                if (!CommandOptions.TryGetValue(subCommandType, out var subCommandOptions))
                {
                    yield break;
                }

                yield return new SlashCommandOptionBuilder
                {
                    Name = subCommandAttribute.Name,
                    Type = ApplicationCommandOptionType.SubCommand,
                    Description = subCommandAttribute.Description,
                    Options = CreateOptions(subCommandOptions).ToList()
                };
            }
        }

        foreach (var nestedType in subCommandTypes)
        {
            var att = nestedType.GetCustomAttribute<DiscordBotSubCommandAttribute>();

            if (att is null)
            {
                continue;
            }

            foreach (var sub in CreateSlashSubCommands(nestedType, nestedType, att))
            {
                yield return sub;
            }
        }
    }

    private IEnumerable<SlashCommandOptionBuilder> CreateOptions(Dictionary<string, PropertyInfo> options)
    {
        foreach (var (optionName, property) in options)
        {
            var option = CreateOption(optionName, property);

            if (option is not null)
            {
                yield return option;
            }
        }
    }

    private SlashCommandOptionBuilder? CreateOption(string optionName, PropertyInfo property)
    {
        if (!CommandOptionAttributes.TryGetValue(property, out var optAtt))
        {
            return null;
        }

        return new SlashCommandOptionBuilder
        {
            Name = optionName,
            Type = optAtt.Type,
            Description = optAtt.Description,
            IsRequired = optAtt.IsRequired,
            IsDefault = optAtt.IsDefault,
            IsAutocomplete = CommandOptionAutocompleteMethods.ContainsKey(property),
            MinValue = optAtt.MinValue,
            MaxValue = optAtt.MaxValue
        };
    }
}