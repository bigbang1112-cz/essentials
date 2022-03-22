using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models.Db;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Data;

public interface IDiscordBotRepo
{
    Task<DiscordBotModel> AddOrUpdateDiscordBotAsync(DiscordBotAttribute attribute, CancellationToken cancellationToken = default);
    Task AddOrUpdateDiscordBotCommandVisibilityAsync(Guid discordBotGuid, SocketTextChannel textChannel, bool set, CancellationToken cancellationToken = default);
    Task<DiscordBotGuildModel> AddOrUpdateDiscordGuildAsync(SocketGuild guild, CancellationToken cancellationToken = default);
    Task<DiscordBotCommandVisibilityModel?> GetDiscordBotCommandVisibilityAsync(DiscordBotJoinedGuildModel joinedGuild, ulong channelSnowflake, CancellationToken cancellationToken = default);
    Task<MemeModel?> GetLastMemeAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default);
    Task<DiscordBotCommandVisibilityModel?> GetDiscordBotCommandVisibilityAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<bool> MemeExistsAsync(string content, CancellationToken cancellationToken = default);
    Task<DiscordBotChannelModel?> GetDiscordChannelAsync(ulong snowflake, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel?> GetJoinedDiscordGuildAsync(DiscordBotModel discordBotModel, SocketGuild guild, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel?> GetJoinedDiscordGuildAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<DiscordBotModel> GetOrAddDiscordBotAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<DiscordBotChannelModel> GetOrAddDiscordChannelAsync(SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<DiscordBotGuildModel> GetOrAddDiscordGuildAsync(SocketGuild guild, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel> GetOrAddJoinedDiscordGuildAsync(DiscordBotModel discordBot, SocketGuild guild, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel> GetOrAddJoinedDiscordGuildAsync(Guid discordBotGuid, SocketGuild guild, CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
    Task AddMemesAsync(IEnumerable<MemeModel> memes, CancellationToken cancellationToken = default);
    Task<List<MemeModel>> GetMemesFromGuildAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default);
    Task AddFeedbackAsync(FeedbackModel feedback, CancellationToken cancellationToken = default);
    Task<MemeModel?> GetRandomMemeAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default);
    Task AddMemeAsync(MemeModel meme, CancellationToken cancellationToken = default);
    Task AddPingMessageAsync(PingMessageModel pingMessage, CancellationToken cancellationToken = default);
    Task<DiscordUserModel> AddOrUpdateDiscordUserAsync(DiscordBotModel bot, SocketUser user, CancellationToken cancellationToken = default);

    async Task<DiscordUserModel> AddOrUpdateDiscordUserAsync(Guid botGuid, SocketUser user, CancellationToken cancellationToken = default)
    {
        var bot = await GetOrAddDiscordBotAsync(botGuid, cancellationToken);
        return await AddOrUpdateDiscordUserAsync(bot, user, cancellationToken);
    }

    Task AddOrUpdateDiscordBotCommandAsync(DiscordBotModel bot, string fullCommandName, CancellationToken cancellationToken = default);

    async Task AddOrUpdateDiscordBotCommandAsync(Guid botGuid, string fullCommandName, CancellationToken cancellationToken = default)
    {
        var bot = await GetOrAddDiscordBotAsync(botGuid, cancellationToken);
        await AddOrUpdateDiscordBotCommandAsync(bot, fullCommandName, cancellationToken);
    }
}
