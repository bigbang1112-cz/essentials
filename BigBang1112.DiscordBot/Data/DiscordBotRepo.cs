using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Extensions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Data;

public class DiscordBotRepo : IDiscordBotRepo
{
    private readonly DiscordBotContext _db;

    public DiscordBotRepo(DiscordBotContext db)
    {
        _db = db;
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

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddMemeAsync(MemeModel meme, CancellationToken cancellationToken = default)
    {
        await _db.Memes.AddAsync(meme, cancellationToken);
    }

    public async Task AddMemesAsync(IEnumerable<MemeModel> memes, CancellationToken cancellationToken = default)
    {
        await _db.Memes.AddRangeAsync(memes, cancellationToken);
    }

    public async Task<List<MemeModel>> GetMemesFromGuildAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default)
    {
        return await _db.Memes.Where(x => x.JoinedGuild == joinedGuild)
            .ToListAsync(cancellationToken);
    }

    public async Task<MemeModel?> GetRandomMemeAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default)
    {
        var skip = Random.Shared.Next(await _db.Memes.CountAsync(cancellationToken));

        return await _db.Memes.OrderBy(x => x.Id)
            .Skip(skip)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
