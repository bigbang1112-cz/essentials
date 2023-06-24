using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Models;

public abstract class GuildCommand : DiscordBotCommand
{
    private readonly IDiscordBotUnitOfWork _discordBotUnitOfWork;

    protected GuildCommand(DiscordBotService discordBotService, IDiscordBotUnitOfWork discordBotUnitOfWork) : base(discordBotService)
    {
        _discordBotUnitOfWork = discordBotUnitOfWork;
    }

    public override async Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand, Deferer deferer)
    {
        var discordBotGuid = GetDiscordBotGuid();

        if (discordBotGuid is null)
        {
            return new DiscordBotMessage("I cannot store anything into a database.", ephemeral: true);
        }

        if (slashCommand.Channel is not SocketTextChannel textChannel)
        {
            return new DiscordBotMessage($"Not a text channel, server cannot be detected ({slashCommand.Channel.GetType().Name}).", ephemeral: true);
        }

        var joinedGuild = await _discordBotUnitOfWork.DiscordBotJoinedGuilds
            .GetByBotAndTextChannelAsync(discordBotGuid.Value, textChannel);

        if (joinedGuild is null)
        {
            return new DiscordBotMessage("Guild couldn't be detected for unknown reason.", ephemeral: true);
        }

        return await ExecuteWithJoinedGuildAsync(slashCommand, deferer, joinedGuild, textChannel);
    }

    public abstract Task<DiscordBotMessage> ExecuteWithJoinedGuildAsync(SocketInteraction slashCommand,
                                                                        Deferer deferer,
                                                                        DiscordBotJoinedGuildModel joinedGuild,
                                                                        SocketTextChannel textChannel);
}
