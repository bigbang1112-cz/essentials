using BigBang1112.Attributes;
using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models;
using BigBang1112.DiscordBot.Models.Db;
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
    private const string ErrorHasOccured = "Error has occured. It has been automatically sent to the bot owner.";

    private readonly DiscordBotAttribute? attribute;
    private ulong ownerDiscordSnowflake;
    private ulong guildDiscordSnowflake;
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
        Client.ButtonExecuted += ButtonExecutedAsync;
        Client.GuildAvailable += GuildAvailableAsync;
        Client.ModalSubmitted += ModalSubmittedAsync;

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

        ownerDiscordSnowflake = _config.GetValue<ulong>("DiscordBotOwner");
        guildDiscordSnowflake = _config.GetValue<ulong>("DiscordGuild");

        var secret = _config[GetType().GetCustomAttribute<SecretAppsettingsPathAttribute>()?.Path ?? throw new Exception()];

        if (string.IsNullOrWhiteSpace(secret))
        {
            return;
        }

        await CreateDiscordBotInDatabase(cancellationToken);

        await Client.LoginAsync(TokenType.Bot, secret);
        await Client.StartAsync();

#if RELEASE
        //await CreateGlobalApplicationCommandsAsync();
#endif
    }

    public Guid? GetGuid()
    {
        return attribute?.Guid;
    }

    public string? GetName()
    {
        return attribute?.Name;
    }

    public string? GetVersion()
    {
        return attribute?.Version;
    }

    public string? GetPunchline()
    {
        return attribute?.Punchline;
    }

    public string? GetDescription()
    {
        return attribute?.Description;
    }

    public string? GetGitRepoUrl()
    {
        return attribute?.GitRepoUrl;
    }

    public ulong GetOwnerDiscordSnowflake()
    {
        return ownerDiscordSnowflake;
    }

    private async Task CreateDiscordBotInDatabase(CancellationToken cancellationToken)
    {
        if (attribute is null)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        var discordBotUnitOfWork = scope.ServiceProvider.GetRequiredService<IDiscordBotUnitOfWork>();

        await discordBotUnitOfWork.DiscordBots.AddOrUpdateAsync(attribute, cancellationToken);

        await discordBotUnitOfWork.SaveAsync(cancellationToken);
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        await Client.LogoutAsync();
    }

    private void CreateCommandDefinitions(Type[] types)
    {
        foreach (var type in types)
        {
            if (!Attribute.IsDefined(type, typeof(DiscordBotCommandAttribute))
             || !type.IsSubclassOf(typeof(DiscordBotCommand))
             || Attribute.IsDefined(type, typeof(UnfinishedDiscordBotCommandAttribute)))
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

            var unfinishedCommand = atts.OfType<UnfinishedDiscordBotCommandAttribute>().FirstOrDefault();

            if (subCommandName is null || unfinishedCommand is not null)
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

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            var autocompleteMethod = commandType.GetMethod($"Autocomplete{property.Name}Async", flags)
                ?? commandType.GetMethod($"Autocomplete{property.Name}", flags);

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

        var discordBotUnitOfWork = scope.ServiceProvider.GetRequiredService<IDiscordBotUnitOfWork>();

        _ = await discordBotUnitOfWork.DiscordBotJoinedGuilds.GetOrAddAsync(attribute.Guid, guild);

        await discordBotUnitOfWork.SaveAsync();

        /*if (guild.Id == guildDiscordSnowflake)
        {
#if DEBUG
            await CreateGuildApplicationCommandsAsync(guild);
#endif
        }*/
    }

    protected virtual async Task SlashCommandExecutedAsync(SocketSlashCommand slashCommand)
    {
        var stopwatch = Stopwatch.StartNew();

        using var scope = CreateCommand(slashCommand, out DiscordBotCommand? command);

        if (command is null)
        {
            return;
        }

        var discordBotUnitOfWork = scope!.ServiceProvider.GetRequiredService<IDiscordBotUnitOfWork>();

        if (attribute is not null)
        {
            var cmdName = GetCommandName(slashCommand);

            await discordBotUnitOfWork.DiscordBotCommands.AddOrUpdateAsync(attribute.Guid, cmdName);
            await discordBotUnitOfWork.DiscordUsers.AddOrUpdateAsync(attribute.Guid, slashCommand.User);
            await discordBotUnitOfWork.SaveAsync();
        }

        var modal = await command.ExecuteModalAsync(slashCommand);

        if (modal is not null)
        {
            await slashCommand.RespondWithModalAsync(modal);
            return;
        }

        var ephemeral = await SetVisibilityOfExecutionAsync(slashCommand, discordBotUnitOfWork);
        var deferer = new Deferer(slashCommand, ephemeral);

        DiscordBotMessage message;

        try
        {
            message = await command.ExecuteAsync(slashCommand, deferer);
        }
        catch (Exception ex)
        {
            if (ex is NotImplementedException)
            {
                await slashCommand.RespondAsync("This command is not yet implemented.", ephemeral: true);
                return;
            }

            var cmdName = GetCommandName(slashCommand);
            
            _logger.LogError(ex, "{botName}: Error has occurred while executing /{command}:", GetName(), cmdName);

            var embed = new EmbedBuilder().WithDescription($"```\n{ex}```").WithColor(255, 0, 0).Build();

            if (deferer.IsDeferred)
            {
                await slashCommand.FollowupAsync(ErrorHasOccured, embed: embed, ephemeral: true);
            }
            else
            {
                await slashCommand.RespondAsync(ErrorHasOccured, embed: embed, ephemeral: true);
            }

            return;
        }

        if (!ephemeral)
        {
            ephemeral = message.Ephemeral;
        }

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

    private static string GetCommandName(SocketSlashCommand slashCommand)
    {
        var cmdName = slashCommand.CommandName;

        var subCommands = RecurseSubCommands(slashCommand.Data.Options);

        if (subCommands.Any())
        {
            return $"{cmdName} {string.Join(' ', subCommands)}";
        }

        return cmdName;
    }

    private async Task<bool> SetVisibilityOfExecutionAsync(SocketSlashCommand slashCommand, bool expectedEphemeral, IDiscordBotUnitOfWork discordBotUnitOfWork)
    {
        if (slashCommand.Channel is not SocketTextChannel textChannel || attribute is null)
        {
            return !slashCommand.IsDMInteraction;
        }

        var joinedGuild = await discordBotUnitOfWork.DiscordBotJoinedGuilds
            .GetByBotAndTextChannelAsync(attribute.Guid, textChannel);

        if (joinedGuild is null)
        {
            return true;
        }

        var visibility = await discordBotUnitOfWork.DiscordBotCommandVisibilities
            .GetByJoinedGuildAndChannelAsync(joinedGuild, textChannel.Id);

        if (visibility is null)
        {
            if (joinedGuild.CommandVisibility)
            {
                return expectedEphemeral;
            }

            return true;
        }

        if (visibility.Visibility || visibility.JoinedGuild.CommandVisibility)
        {
            return expectedEphemeral;
        }

        return true;
    }

    private async Task<bool> SetVisibilityOfExecutionAsync(SocketSlashCommand slashCommand, IDiscordBotUnitOfWork discordBotUnitOfWork)
    {
        return await SetVisibilityOfExecutionAsync(slashCommand, expectedEphemeral: false, discordBotUnitOfWork);
    }

    private async Task<bool> SetVisibilityOfExecutionAsync(SocketSlashCommand slashCommand, DiscordBotMessage message, IDiscordBotUnitOfWork discordBotUnitOfWork)
    {
        return await SetVisibilityOfExecutionAsync(slashCommand, message.Ephemeral, discordBotUnitOfWork);
    }

    protected virtual async Task ProcessInteractionAsync(SocketMessageComponent messageComponent, Func<DiscordBotCommand, SocketMessageComponent, Deferer, Task<DiscordBotMessage?>> func)
    {
        var stopwatch = Stopwatch.StartNew();

        using var scope = CreateCommand(messageComponent, out DiscordBotCommand? command, out bool isAutomatic);

        // cannot be processed if command is not null and is automatic at the same time
        if (command is not null && isAutomatic)
        {
            return;
        }

        var deferer = new Deferer(messageComponent, false);

        DiscordBotMessage? message;

        try
        {
            if (isAutomatic)
            {
                message = await ProcessInteractionOnAutomaticMessageAsync(messageComponent, deferer);
            }
            else if (command is not null)
            {
                message = await func(command, messageComponent, deferer);
            }
            else
            {
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{botName}: Error while interacting with a component:", GetName());
            return;
        }

        if (message is null)
        {
            return;
        }

        if (message.Attachment.HasValue)
        {
            if (message.AlwaysPostAsNewMessage)
            {
                if (deferer.IsDeferred)
                {
                    await messageComponent.FollowupWithFileAsync(message.Attachment.Value,
                        message.Message ?? GetExecutedInMessage(stopwatch), message.Embeds,
                        ephemeral: message.Ephemeral, components: message.Component);
                }
                else
                {
                    await messageComponent.RespondWithFileAsync(message.Attachment.Value,
                        message.Message ?? GetExecutedInMessage(stopwatch), message.Embeds,
                        ephemeral: message.Ephemeral, components: message.Component);
                }

                return;
            }

            if (!deferer.IsDeferred)
            {
                await messageComponent.DeferAsync(message.Ephemeral);
            }

            await messageComponent.ModifyOriginalResponseAsync(x =>
            {
                x.Content = message.Message ?? GetExecutedInMessage(stopwatch);
                x.Embeds = message.Embeds;

                if (message.Component is not null)
                {
                    x.Components = message.Component;
                }

                x.Attachments = new Optional<IEnumerable<FileAttachment>>(Enumerable.Repeat(message.Attachment.Value, 1));
            });

            return;
        }

        if (message.AlwaysPostAsNewMessage)
        {
            if (deferer.IsDeferred)
            {
                await messageComponent.FollowupAsync(message.Message ?? GetExecutedInMessage(stopwatch),
                    message.Embeds, ephemeral: message.Ephemeral, components: message.Component);
            }
            else
            {
                await messageComponent.RespondAsync(message.Message ?? GetExecutedInMessage(stopwatch),
                    message.Embeds, ephemeral: message.Ephemeral, components: message.Component);
            }

            return;
        }

        if (deferer.IsDeferred)
        {
            await messageComponent.ModifyOriginalResponseAsync(x =>
            {
                x.Content = message.Message ?? GetExecutedInMessage(stopwatch);
                x.Embeds = message.Embeds;

                if (message.Component is not null)
                {
                    x.Components = message.Component;
                }
            });
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

    protected virtual Task<DiscordBotMessage?> ProcessInteractionOnAutomaticMessageAsync(SocketMessageComponent messageComponent, Deferer deferer)
    {
        return messageComponent.Data.Type switch
        {
            ComponentType.Button => ButtonExecutedOnAutomaticMessageAsync(messageComponent, deferer),
            _ => Task.FromResult(default(DiscordBotMessage)),
        };
    }

    protected virtual Task<DiscordBotMessage?> ButtonExecutedOnAutomaticMessageAsync(SocketMessageComponent messageComponent, Deferer deferer)
    {
        return Task.FromResult(default(DiscordBotMessage));
    }

    protected virtual async Task SelectMenuExecutedAsync(SocketMessageComponent messageComponent)
    {
        await ProcessInteractionAsync(messageComponent, (command, component, deferer) => command.SelectMenuAsync(component, deferer));
    }

    protected virtual async Task ButtonExecutedAsync(SocketMessageComponent messageComponent)
    {
        await ProcessInteractionAsync(messageComponent, (command, component, deferer) => command.ExecuteButtonAsync(component, deferer));
    }

    protected virtual async Task ModalSubmittedAsync(SocketModal modal)
    {
        if (modal.Data.CustomId == "feedback-modal")
        {
            await ProcessFeedbackModalAsync(modal);
        }
    }

    private async Task ProcessFeedbackModalAsync(SocketModal modal)
    {
        var messageComponent = modal.Data.Components.First(x => x.CustomId == "feedback-text");
        var message = messageComponent.Value;

        var guid = GetGuid() ?? throw new Exception();

        using var scope = _serviceProvider.CreateScope();

        var discordBotUnitOfWork = scope.ServiceProvider.GetRequiredService<IDiscordBotUnitOfWork>();

        var user = await discordBotUnitOfWork.DiscordUsers.AddOrUpdateAsync(guid, modal.User);

        if (user.IsBlocked)
        {
            await modal.RespondAsync(ephemeral: true,
                embed: new EmbedBuilder().WithTitle("Your feedback has not been submitted. You're blocked.").Build());
            return;
        }

        var feedback = new FeedbackModel
        {
            Text = message,
            User = user,
            WrittenOn = DateTime.UtcNow
        };

        await discordBotUnitOfWork.Feedbacks.AddAsync(feedback);
        await discordBotUnitOfWork.SaveAsync();

        await modal.RespondAsync(ephemeral: true,
            embed: new EmbedBuilder().WithTitle("Your feedback has been successfully submitted.").Build());

        var feedbackReceiver = await Client.GetUserAsync(ownerDiscordSnowflake);

        var embed = new EmbedBuilder()
            .WithTitle("Feedback")
            .WithDescription(message)
            .WithAuthor(modal.User)
            .AddField("User", modal.User.Id)
            .Build();

        await feedbackReceiver.SendMessageAsync(embed: embed);
    }

    protected virtual async Task AutocompleteExecutedAsync(SocketAutocompleteInteraction interaction)
    {
        var commandType = GetCommandType(interaction);

        if (commandType is null)
        {
            return;
        }

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

        var parameters = autocompleteMethod.GetParameters();

        var objParams = new object?[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            objParams[i] = parameters[i].ParameterType switch
            {
                Type strType when strType == typeof(string) => option.Value,
                Type interactionType when interactionType == typeof(SocketAutocompleteInteraction) => interaction,
                _ => null,
            };
        }
        
        var autocompleteOptions = autocompleteMethod.Invoke(command, objParams) switch
        {
            Task<IEnumerable<string>> task => await task,
            IEnumerable<string> directOptions => directOptions,
            _ => null
        };

        if (autocompleteOptions is null)
        {
            await interaction.RespondAsync(null);
            return;
        }

        await interaction.RespondAsync(autocompleteOptions.Select(x => new AutocompleteResult(x, x)));
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

        if (type is null)
        {
            command = null;
            return null;
        }

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

    private IServiceScope? CreateCommand(SocketMessageComponent messageComponent, out DiscordBotCommand? command, out bool isAutomatic)
    {
        var split = messageComponent.Data.CustomId.Split('-');

        if (split.Length == 0)
        {
            command = null;
            isAutomatic = false;
            return null;
        }

        var commandName = split[0].Replace('_', ' ');

        var type = GetCommandType(commandName);

        if (type is null)
        {
            command = null;
            isAutomatic = true;
            return null;
        }

        isAutomatic = false;
        return CreateCommand(type, out command);
    }

    private IServiceScope? CreateCommand(SocketAutocompleteInteraction interaction, out DiscordBotCommand? command, out bool isAutomatic)
    {
        var type = GetCommandType(interaction);

        if (type is null)
        {
            command = null;
            isAutomatic = true;
            return null;
        }

        isAutomatic = false;
        return CreateCommand(type, out command);
    }

    public IServiceScope? CreateCommand(Type commandType, out DiscordBotCommand? command)
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

    public IServiceScope? CreateCommand<T>(out T? command) where T : DiscordBotCommand
    {
        var scope = CreateCommand(typeof(T), out DiscordBotCommand? cmd);
        command = cmd as T;
        return scope;
    }

    private Type? GetCommandType(SocketSlashCommand slashCommand)
    {
        return GetCommandType(slashCommand.CommandName, slashCommand.Data.Options);
    }

    private Type? GetCommandType(SocketAutocompleteInteraction interaction)
    {
        return GetCommandType(interaction.Data.CommandName, interaction.Data.Options);
    }

    private Type? GetCommandType(string commandName, IEnumerable<AutocompleteOption> options)
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

    private Type? GetCommandType(string commandName, IEnumerable<IApplicationCommandInteractionDataOption> options)
    {
        var subCommands = RecurseSubCommands(options);

        if (subCommands.Any())
        {
            commandName = $"{commandName} {string.Join(' ', subCommands)}";
        }

        return GetCommandType(commandName);
    }

    private static IEnumerable<string> RecurseSubCommands(IEnumerable<IApplicationCommandInteractionDataOption> options)
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

    private Type? GetCommandType(string commandName)
    {
        if (commandName == " automatic")
        {
            return null;
        }

        if (Commands.TryGetValue(commandName, out Type? commandType))
        {
            return commandType;
        }

        throw new NotImplementedException();
    }

    protected virtual async Task MessageReceivedAsync(SocketMessage msg)
    {
        if (msg.Channel is not IDMChannel && msg.MentionedUsers.Any(x => x.Id == Client.CurrentUser.Id))
        {
            await StorePingMessageAsync(msg);
        }

        if (msg.Author.Id != ownerDiscordSnowflake)
        {
            return;
        }

        if (msg is not SocketUserMessage userMsg)
        {
            return;
        }

        if (userMsg.ReferencedMessage is null)
        {
            return;
        }

        var embed = userMsg.ReferencedMessage.Embeds.FirstOrDefault();

        if (embed is null)
        {
            return;
        }

        if (embed.Title != "Feedback")
        {
            return;
        }

        if (embed.Fields.Length == 0)
        {
            return;
        }

        var field = embed.Fields[0];

        if (field.Name != "User")
        {
            return;
        }

        if (!ulong.TryParse(field.Value, out ulong discordUserSnowflake))
        {
            return;
        }

        var user = await Client.GetUserAsync(discordUserSnowflake);

        var responseEmbed = new EmbedBuilder()
            .WithTitle("Response to your feedback")
            .WithDescription(msg.Content)
            .WithAuthor(msg.Author)
            .Build();

        var noteEmbed = new EmbedBuilder()
            .WithTitle("Do not respond to this message here!")
            .WithDescription($"{GetName()} DMs are **not** exposed to the bot owner.\nContact {msg.Author.Mention} instead if you want to discuss the feedback more.")
            .WithColor(255, 255, 0)
            .Build();

        await user.SendMessageAsync(embeds: new Embed[] { responseEmbed, noteEmbed });
    }

    private async Task StorePingMessageAsync(SocketMessage msg)
    {
        using var scope = _serviceProvider.CreateScope();

        var discordBotUnitOfWork = scope.ServiceProvider.GetRequiredService<IDiscordBotUnitOfWork>();

        var guid = GetGuid() ?? throw new Exception();

        var user = await discordBotUnitOfWork.DiscordUsers.AddOrUpdateAsync(guid, msg.Author);

        var pingMessage = new PingMessageModel
        {
            Text = msg.CleanContent,
            User = user,
            WrittenOn = DateTime.UtcNow
        };

        await discordBotUnitOfWork.PingMessages.AddAsync(pingMessage);
        await discordBotUnitOfWork.SaveAsync();
    }

    private Task LogAsync(LogMessage arg)
    {
        switch (arg.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical(arg.Exception, "{name}: {message}", GetName(), arg.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError(arg.Exception, "{name}: {message}", GetName(), arg.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning(arg.Exception, "{name}: {message}", GetName(), arg.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("{name}: {message}", GetName(), arg.Message);
                break;
        }

        return Task.CompletedTask;
    }

    protected virtual Task ReadyAsync()
    {
        return Task.CompletedTask;
    }

    public async Task CreateGuildApplicationCommandsAsync(SocketGuild guild)
    {
        var commands = BuildApplicationCommands();

        foreach (var command in commands)
        {
            await guild.CreateApplicationCommandAsync(command);
        }
    }

    public async Task CreateGlobalApplicationCommandsAsync()
    {
        var commands = BuildApplicationCommands();

        foreach (var command in commands)
        {
            await Client.CreateGlobalApplicationCommandAsync(command);
        }
    }

    private IEnumerable<SlashCommandProperties> BuildApplicationCommands()
    {
        var commandBuilders = CreateSlashCommands();
        var commands = BuildSlashCommands(commandBuilders);
        return commands;
    }

    public async Task OverwriteGuildApplicationCommandsAsync()
    {
        var guild = Client.GetGuild(_config.GetValue<ulong>("DiscordGuild"));

        if (guild is null)
        {
            return;
        }

        await OverwriteGuildApplicationCommandsAsync(guild);
    }

    public async Task OverwriteGuildApplicationCommandsAsync(SocketGuild guild)
    {
        var commandBuilders = CreateSlashCommands();
        var commands = BuildSlashCommands(commandBuilders);

        await guild.BulkOverwriteApplicationCommandAsync(commands.ToArray());
        //await Client.BulkOverwriteGlobalApplicationCommandsAsync(commands.ToArray());
    }

    public async Task ClearGuildApplicationCommandsAsync()
    {
        var guild = Client.GetGuild(_config.GetValue<ulong>("DiscordGuild"));

        if (guild is null)
        {
            return;
        }

        await ClearGuildApplicationCommandsAsync(guild);
    }

    public static async Task ClearGuildApplicationCommandsAsync(SocketGuild guild)
    {
        await guild.BulkOverwriteApplicationCommandAsync(Array.Empty<ApplicationCommandProperties>());
        //await Client.BulkOverwriteGlobalApplicationCommandsAsync(commands.ToArray());
    }

    public async Task OverwriteGlobalApplicationCommandsAsync()
    {
        var commandBuilders = CreateSlashCommands();
        var commands = BuildSlashCommands(commandBuilders);

        await Client.BulkOverwriteGlobalApplicationCommandsAsync(commands.ToArray());
    }

    public async Task ClearGlobalApplicationCommandsAsync()
    {
        await Client.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>());
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

            _logger.LogInformation("All commands of '/{commandName}' created.", commandName);

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
            _logger.LogInformation("Subcommand '{name}' created.", subCommandAttribute.Name);

            if (subCommandTypes.Count > 0)
            {
                var subCommands = CreateSlashSubCommands(subCommandType, null, null).ToList();

                if (subCommands.Count == 0)
                {
                    _logger.LogWarning("{commandType} has no options.", commandType);
                    yield break;
                }

                yield return new SlashCommandOptionBuilder
                {
                    Name = subCommandAttribute.Name,
                    Type = ApplicationCommandOptionType.SubCommandGroup,
                    Description = subCommandAttribute.Description,
                    Options = subCommands
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
            MinValue = optAtt.MinValue == double.MinValue ? null : optAtt.MinValue,
            MaxValue = optAtt.MaxValue == double.MaxValue ? null : optAtt.MaxValue,
            Choices = optAtt.Choices?.Select((name, i) => new ApplicationCommandOptionChoiceProperties
            {
                Name = name,
                Value = i
            }).ToList()
        };
    }

    /// <summary>
    /// Sends a tracked text message to an allowed text channel. Usage of this method should have a report context, and shouldn't be used for sending random messages.
    /// </summary>
    /// <param name="discordBotUnitOfWork">Data access used to save the message.</param>
    /// <param name="reportChannel">Channel that allows reporting.</param>
    /// <param name="message">Creation of the message data for the database, providing a message snowflake as <see cref="ulong"/>.</param>
    /// <param name="text">Text to send in the message.</param>
    /// <param name="embeds">Embeds to send in the message.</param>
    /// <param name="components">Components to send in the message.</param>
    /// <param name="autoThread">Thread to automatically create on the message; null won't create any thread</param>
    /// <param name="requestOptions">Request options.</param>
    /// <returns>A message of a report kind.</returns>
    /// <exception cref="ArgumentNullException">discordBotUnitOfWork or reportChannel cannot be null.</exception>
    public async Task<ReportChannelMessageModel?> SendMessageAsync(IDiscordBotUnitOfWork discordBotUnitOfWork,
                                                                   ReportChannelModel reportChannel,
                                                                   Func<ulong, ReportChannelMessageModel>? message = null,
                                                                   string? text = null,
                                                                   IEnumerable<Embed>? embeds = null,
                                                                   MessageComponent? components = null,
                                                                   AutoThread? autoThread = null,
                                                                   RequestOptions? requestOptions = default)
    {
        _ = discordBotUnitOfWork ?? throw new ArgumentNullException(nameof(discordBotUnitOfWork));
        _ = reportChannel ?? throw new ArgumentNullException(nameof(reportChannel));

        var guild = Client.GetGuild(reportChannel.JoinedGuild.Guild.Snowflake);

        if (guild is null)
        {
            return null;
        }
        
        var channel = guild.GetTextChannel(reportChannel.Channel.Snowflake);
        
        if (channel is null)
        {
            return null;
        }

        var restMessage = await channel.SendMessageAsync(text, components: components, embeds: embeds?.ToArray());

        if (restMessage is null)
        {
            return null;
        }
        
        if (autoThread is not null)
        {
            try
            {
                _ = await channel.CreateThreadAsync(autoThread.Name,
                    autoArchiveDuration: autoThread.Options.ArchiveDuration,
                    message: restMessage, options: requestOptions);
            }
            catch (Discord.Net.HttpException ex)
            {
                _logger.LogWarning(ex, "Failed to create a thread from the report with TMWR bot to #{channelName} on {guildName} guild due to Discord API.", reportChannel.Channel.Name, reportChannel.JoinedGuild.Guild.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a thread from the report with TMWR bot to #{channelName} on {guildName} guild.", reportChannel.Channel.Name, reportChannel.JoinedGuild.Guild.Name);
            }
        }

        if (message is null)
        {
            return null;
        }

        var msg = message.Invoke(restMessage.Id);

        await discordBotUnitOfWork.ReportChannelMessages.AddAsync(msg, requestOptions?.CancelToken ?? default);

        return msg;
    }

    public async Task ModifyMessageAsync(ReportChannelMessageModel msg,
                                         string? text = null,
                                         IEnumerable<Embed>? embeds = null)
    {
        var guild = Client.GetGuild(msg.Channel.JoinedGuild.Guild.Snowflake);

        if (guild is null)
        {
            msg.RemovedByUser = true;
            return;
        }

        var channel = guild.GetTextChannel(msg.Channel.Channel.Snowflake);

        if (channel is null)
        {
            msg.RemovedByUser = true;
            return;
        }

        msg.ModifiedOn = DateTime.UtcNow;

        try
        {
            await channel.ModifyMessageAsync(msg.MessageId, x =>
            {
                if (text is not null)
                {
                    x.Content = text;
                }

                if (embeds is not null)
                {
                    x.Embeds = new(embeds.ToArray());
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Message (DB ID: {msgId}) couldn't be modified.", msg.Id);
        }
    }

    public async Task DeleteMessageAsync(ReportChannelMessageModel msg)
    {
        var guild = Client.GetGuild(msg.Channel.JoinedGuild.Guild.Snowflake);

        if (guild is null)
        {
            msg.RemovedByUser = true;
            return;
        }

        var channel = guild.GetTextChannel(msg.Channel.Channel.Snowflake);

        if (channel is null)
        {
            msg.RemovedByUser = true;
            return;
        }

        try
        {
            await channel.DeleteMessageAsync(msg.MessageId);
            msg.RemovedOfficially = true;
        }
        catch (Discord.Net.HttpException httpEx)
        {
            if (httpEx.HttpCode == System.Net.HttpStatusCode.NotFound
            && !msg.RemovedOfficially)
            {
                msg.RemovedByUser = true;
            }
        }
    }

    public static string CreateCustomId(string? additional = null)
    {
        var customId = string.IsNullOrWhiteSpace(additional) ? "_automatic" : $"_automatic-{additional}";

        return customId.ToLower().Replace(' ', '_');
    }
}
