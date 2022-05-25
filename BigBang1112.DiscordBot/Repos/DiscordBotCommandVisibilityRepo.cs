using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class DiscordBotCommandVisibilityRepo : Repo<DiscordBotCommandVisibilityModel>, IDiscordBotCommandVisibilityRepo
{
    private readonly DiscordBotContext _context;

    public DiscordBotCommandVisibilityRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<DiscordBotCommandVisibilityModel?> GetByJoinedGuildAndChannelAsync(DiscordBotJoinedGuildModel joinedGuild, ulong channelSnowflake, CancellationToken cancellationToken = default)
    {
        return await _context.DiscordBotCommandVisibilities
            .Include(x => x.Channel)
            .Include(x => x.JoinedGuild)
            .FirstOrDefaultAsync(x => x.Channel.Snowflake == channelSnowflake && x.JoinedGuild == joinedGuild,
                cancellationToken);
    }

    public async Task<DiscordBotCommandVisibilityModel?> GetByBotAndTextChannelAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default)
    {
        var joinedGuild = await new DiscordBotJoinedGuildRepo(_context).GetByBotAndTextChannelAsync(discordBotGuid, textChannel, cancellationToken);

        if (joinedGuild is null)
        {
            return null;
        }

        return await GetByJoinedGuildAndChannelAsync(joinedGuild, textChannel.Id, cancellationToken);
    }

    public async Task AddOrUpdateAsync(Guid discordBotGuid, SocketTextChannel textChannel, bool set, CancellationToken cancellationToken = default)
    {
        var joinedGuild = await new DiscordBotJoinedGuildRepo(_context).GetByBotAndTextChannelAsync(discordBotGuid, textChannel, cancellationToken);

        if (joinedGuild is null)
        {
            return;
        }

        var visibiltyModel = await GetByJoinedGuildAndChannelAsync(joinedGuild, textChannel.Id, cancellationToken);

        if (visibiltyModel is null)
        {
            var channel = await new DiscordBotChannelRepo(_context).GetOrAddAsync(textChannel, cancellationToken);

            visibiltyModel = new DiscordBotCommandVisibilityModel
            {
                JoinedGuild = joinedGuild,
                Channel = channel
            };

            await _context.DiscordBotCommandVisibilities.AddAsync(visibiltyModel, cancellationToken);
        }

        visibiltyModel.Visibility = set;
    }
}
