using BigBang1112.Attributes.DiscordBot;
using BigBang1112.Models.Db;
using Discord.WebSocket;

namespace BigBang1112.Data;

public interface IAccountsRepo
{
    Task AddAccountAsync(AccountModel account, CancellationToken cancellationToken = default);
    Task AddManiaPlanetAuthAsync(ManiaPlanetAuthModel maniaPlanetAuth, CancellationToken cancellationToken = default);
    Task<DiscordBotModel> AddOrUpdateDiscordBotAsync(DiscordBotAttribute attribute, CancellationToken cancellationToken = default);
    Task AddOrUpdateDiscordBotCommandVisibilityAsync(Guid discordBotGuid, SocketTextChannel textChannel, bool set, CancellationToken cancellationToken = default);
    Task<DiscordBotGuildModel> AddOrUpdateDiscordGuildAsync(SocketGuild guild, CancellationToken cancellationToken = default);
    AccountModel CreateNewAccount();
    Task<AccountModel?> GetAccountByGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<DiscordBotCommandVisibilityModel?> GetDiscordBotCommandVisibilityAsync(DiscordBotJoinedGuildModel joinedGuild, ulong channelSnowflake, CancellationToken cancellationToken = default);
    Task<DiscordBotCommandVisibilityModel?> GetDiscordBotCommandVisibilityAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<DiscordBotChannelModel?> GetDiscordChannelAsync(ulong snowflake, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel?> GetJoinedDiscordGuildAsync(DiscordBotModel discordBotModel, SocketGuild guild, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel?> GetJoinedDiscordGuildAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<List<ManiaPlanetLbManialinkModel>> GetLbManialinkMembersAsync(CancellationToken cancellationToken = default);
    Task<ManiaPlanetAuthModel?> GetManiaPlanetAuthByAccessTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<ManiaPlanetAuthModel?> GetManiaPlanetAuthByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<DiscordAuthModel> GetOrAddDiscordAuthAsync(ulong snowflake, CancellationToken cancellationToken = default);
    Task<DiscordBotModel> GetOrAddDiscordBotAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<DiscordBotChannelModel> GetOrAddDiscordChannelAsync(SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<DiscordBotGuildModel> GetOrAddDiscordGuildAsync(SocketGuild guild, CancellationToken cancellationToken = default);
    Task<GitHubAuthModel> GetOrAddGitHubAuthAsync(uint uid, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel> GetOrAddJoinedDiscordGuildAsync(DiscordBotModel discordBot, SocketGuild guild, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel> GetOrAddJoinedDiscordGuildAsync(Guid discordBotGuid, SocketGuild guild, CancellationToken cancellationToken = default);
    Task<ManiaPlanetAuthModel> GetOrAddManiaPlanetAuthAsync(string login, CancellationToken cancellationToken = default);
    Task<TrackmaniaAuthModel> GetOrAddTrackmaniaAuthAsync(Guid loginGuid, CancellationToken cancellationToken = default);
    Task<TwitterAuthModel> GetOrAddTwitterAuthAsync(ulong userId, CancellationToken cancellationToken = default);
    Task<ZoneModel> GetOrAddZoneAsync(string zone, CancellationToken cancellationToken = default);
    Task<bool> IsAdminAsync(AccountModel account, CancellationToken cancellationToken = default);
    void RemoveAccount(AccountModel account);
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
}
