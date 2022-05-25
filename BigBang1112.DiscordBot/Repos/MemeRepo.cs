using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class MemeRepo : Repo<MemeModel>, IMemeRepo
{
    private readonly DiscordBotContext _context;

    public MemeRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MemeModel>> GetFromGuildAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default)
    {
        return await _context.Memes
            .Where(x => x.JoinedGuild == joinedGuild)
            .ToListAsync(cancellationToken);
    }

    public async Task<MemeModel?> GetRandomAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default)
    {
        var skip = Random.Shared.Next(await _context.Memes.CountAsync(cancellationToken));

        return await _context.Memes
            .Where(x => x.JoinedGuild == joinedGuild)
            .OrderBy(x => x.Id)
            .Skip(skip)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MemeModel?> GetLastAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default)
    {
        return await _context.Memes
            .Where(x => x.JoinedGuild == joinedGuild)
            .OrderByDescending(x => x.AddedOn)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string content, CancellationToken cancellationToken = default)
    {
        return await _context.Memes
            .AnyAsync(x => x.Content.ToLower() == content.ToLower(), cancellationToken);
    }
}
