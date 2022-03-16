using BigBang1112.Attributes.DiscordBot;
using BigBang1112.Extensions;
using BigBang1112.Models.Db;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.Data;

public class AccountsRepo : IAccountsRepo
{
    private readonly AccountsContext _db;

    public AccountsRepo(AccountsContext db)
    {
        _db = db;
    }

    public async Task<ManiaPlanetAuthModel?> GetManiaPlanetAuthByAccessTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _db.ManiaPlanetAuth
            .Include(x => x.LbManialink)
            .FirstOrDefaultAsync(x => x.AccessToken == token, cancellationToken);
    }

    public async Task<ManiaPlanetAuthModel?> GetManiaPlanetAuthByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _db.ManiaPlanetAuth.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
    }

    public async Task AddManiaPlanetAuthAsync(ManiaPlanetAuthModel maniaPlanetAuth, CancellationToken cancellationToken = default)
    {
        await _db.ManiaPlanetAuth.AddAsync(maniaPlanetAuth, cancellationToken);
    }

    public async Task AddAccountAsync(AccountModel account, CancellationToken cancellationToken = default)
    {
        await _db.Accounts.AddAsync(account, cancellationToken);
    }

    public async Task<ZoneModel> GetOrAddZoneAsync(string zone, CancellationToken cancellationToken = default)
    {
        return await _db.Zones.FirstOrAddAsync(x => x.Name == zone, () => new ZoneModel
        {
            Name = zone
        }, cancellationToken: cancellationToken);
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ManiaPlanetLbManialinkModel>> GetLbManialinkMembersAsync(CancellationToken cancellationToken = default)
    {
        return await _db.LbManialink
            .Include(x => x.Auth)
                .ThenInclude(x => x.Zone)
            .OrderByDescending(x => x.LastVisitedOn)
            .ToListAsync(cancellationToken);
    }

    public AccountModel CreateNewAccount() => new()
    {
        Guid = Guid.NewGuid(),
        CreatedOn = DateTime.UtcNow,
        LastSeenOn = DateTime.UtcNow
    };

    public async Task<TrackmaniaAuthModel> GetOrAddTrackmaniaAuthAsync(Guid loginGuid, CancellationToken cancellationToken = default)
    {
        var auth = await _db.TrackmaniaAuth.FirstOrAddAsync(x => x.Login == loginGuid, () => new TrackmaniaAuthModel
        {
            Account = CreateNewAccount(),
            Login = loginGuid
        }, cancellationToken: cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<ManiaPlanetAuthModel> GetOrAddManiaPlanetAuthAsync(string login, CancellationToken cancellationToken = default)
    {
        var auth = await _db.ManiaPlanetAuth.FirstOrAddAsync(x => x.Login == login, () => new ManiaPlanetAuthModel
        {
            Account = CreateNewAccount(),
            Login = login
        }, cancellationToken: cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<DiscordAuthModel> GetOrAddDiscordAuthAsync(ulong snowflake, CancellationToken cancellationToken = default)
    {
        var auth = await _db.DiscordAuth.FirstOrAddAsync(x => x.Snowflake == snowflake, () => new DiscordAuthModel
        {
            Account = CreateNewAccount(),
            Snowflake = snowflake
        }, cancellationToken: cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<GitHubAuthModel> GetOrAddGitHubAuthAsync(uint uid, CancellationToken cancellationToken = default)
    {
        var auth = await _db.GitHubAuth.FirstOrAddAsync(x => x.Uid == uid, () => new GitHubAuthModel
        {
            Account = CreateNewAccount(),
            Uid = uid
        }, cancellationToken: cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<TwitterAuthModel> GetOrAddTwitterAuthAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        var auth = await _db.TwitterAuth.FirstOrAddAsync(x => x.UserId == userId, () => new TwitterAuthModel
        {
            Account = CreateNewAccount(),
            UserId = userId
        }, cancellationToken: cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<bool> IsAdminAsync(AccountModel account, CancellationToken cancellationToken = default)
    {
        return await _db.Admins.AnyAsync(x => x.Account == account, cancellationToken);
    }

    public void RemoveAccount(AccountModel account)
    {
        _ = _db.Accounts.Remove(account);
    }

    public async Task<AccountModel?> GetAccountByGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await _db.Accounts.FirstOrDefaultAsync(x => x.Guid == guid, cancellationToken);
    }

    public async Task<DiscordBotChannelModel?> GetDiscordChannelAsync(ulong snowflake, CancellationToken cancellationToken = default)
    {
        return await _db.DiscordBotChannels.FirstOrDefaultAsync(x => x.Snowflake == snowflake, cancellationToken);
    }

    public async Task<DiscordBotModel> GetOrAddDiscordBotAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await _db.DiscordBots.FirstOrAddAsync(x => x.Guid == guid,
            () => new DiscordBotModel { Guid = guid }, cancellationToken: cancellationToken);
    }

    public async Task<DiscordBotModel> AddOrUpdateDiscordBotAsync(DiscordBotAttribute attribute, CancellationToken cancellationToken = default)
    {
        var discordBotModel = await GetOrAddDiscordBotAsync(attribute.Guid, cancellationToken);

        discordBotModel.Name = attribute.Name;

        return discordBotModel;
    }

    public async Task<DiscordBotGuildModel> GetOrAddDiscordGuildAsync(SocketGuild guild, CancellationToken cancellationToken = default)
    {
        return await _db.DiscordBotGuilds.FirstOrAddAsync(x => x.Snowflake == guild.Id,
            () => new DiscordBotGuildModel { Snowflake = guild.Id, Name = guild.Name },
            cancellationToken: cancellationToken);
    }

    public async Task<DiscordBotGuildModel> AddOrUpdateDiscordGuildAsync(SocketGuild guild, CancellationToken cancellationToken = default)
    {
        var discordGuildModel = await GetOrAddDiscordGuildAsync(guild, cancellationToken);

        discordGuildModel.Name = guild.Name;

        return discordGuildModel;
    }

    public async Task<DiscordBotChannelModel> GetOrAddDiscordChannelAsync(SocketTextChannel textChannel, CancellationToken cancellationToken = default)
    {
        var discordGuildModel = await GetOrAddDiscordGuildAsync(textChannel.Guild, cancellationToken);

        return await _db.DiscordBotChannels.FirstOrAddAsync(x => x.Snowflake == textChannel.Id,
            () => new DiscordBotChannelModel { Snowflake = textChannel.Id, Name = textChannel.Name, Guild = discordGuildModel },
            cancellationToken: cancellationToken);
    }

    public async Task<DiscordBotCommandVisibilityModel?> GetDiscordBotCommandVisibilityAsync(DiscordBotJoinedGuildModel joinedGuild, ulong channelSnowflake, CancellationToken cancellationToken = default)
    {
        return await _db.DiscordBotCommandVisibilities
            .Include(x => x.Channel)
            .Include(x => x.JoinedGuild)
            .FirstOrDefaultAsync(x => x.Channel.Snowflake == channelSnowflake && x.JoinedGuild == joinedGuild,
                cancellationToken);
    }

    public async Task<DiscordBotCommandVisibilityModel?> GetDiscordBotCommandVisibilityAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default)
    {
        var joinedGuild = await GetJoinedDiscordGuildAsync(discordBotGuid, textChannel, cancellationToken);

        if (joinedGuild is null)
        {
            return null;
        }

        return await GetDiscordBotCommandVisibilityAsync(joinedGuild, textChannel.Id, cancellationToken);
    }

    public async Task<DiscordBotJoinedGuildModel?> GetJoinedDiscordGuildAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default)
    {
        var discordBotModel = await GetOrAddDiscordBotAsync(discordBotGuid, cancellationToken);

        if (discordBotModel is null)
        {
            return null;
        }

        return await GetJoinedDiscordGuildAsync(discordBotModel, textChannel.Guild, cancellationToken);
    }

    public async Task<DiscordBotJoinedGuildModel?> GetJoinedDiscordGuildAsync(DiscordBotModel discordBotModel, SocketGuild guild, CancellationToken cancellationToken = default)
    {
        return await _db.DiscordBotJoinedGuilds.FirstOrDefaultAsync(x => x.Bot == discordBotModel && x.Guild.Snowflake == guild.Id, cancellationToken);
    }

    public async Task AddOrUpdateDiscordBotCommandVisibilityAsync(Guid discordBotGuid, SocketTextChannel textChannel, bool set, CancellationToken cancellationToken = default)
    {
        var joinedGuild = await GetJoinedDiscordGuildAsync(discordBotGuid, textChannel, cancellationToken);

        if (joinedGuild is null)
        {
            return;
        }

        var visibiltyModel = await GetDiscordBotCommandVisibilityAsync(joinedGuild, textChannel.Id, cancellationToken);

        if (visibiltyModel is null)
        {
            var channel = await GetOrAddDiscordChannelAsync(textChannel, cancellationToken);

            visibiltyModel = new DiscordBotCommandVisibilityModel
            {
                JoinedGuild = joinedGuild,
                Channel = channel
            };

            await _db.DiscordBotCommandVisibilities.AddAsync(visibiltyModel, cancellationToken);
        }

        visibiltyModel.Visibility = set;
    }

    public async Task<DiscordBotJoinedGuildModel> GetOrAddJoinedDiscordGuildAsync(DiscordBotModel discordBot, SocketGuild guild, CancellationToken cancellationToken = default)
    {
        var discordGuildModel = await AddOrUpdateDiscordGuildAsync(guild, cancellationToken);

        return await _db.DiscordBotJoinedGuilds.FirstOrAddAsync(x => x.Guild.Id == discordGuildModel.Id,
            () => new DiscordBotJoinedGuildModel { Bot = discordBot, Guild = discordGuildModel },
            cancellationToken: cancellationToken);
    }

    public async Task<DiscordBotJoinedGuildModel> GetOrAddJoinedDiscordGuildAsync(Guid discordBotGuid, SocketGuild guild, CancellationToken cancellationToken = default)
    {
        var discordBot = await GetOrAddDiscordBotAsync(discordBotGuid, cancellationToken);

        return await GetOrAddJoinedDiscordGuildAsync(discordBot, guild, cancellationToken);
    }
}
