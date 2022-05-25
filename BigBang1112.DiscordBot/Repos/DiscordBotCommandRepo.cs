using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Extensions;
using BigBang1112.Repos;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class DiscordBotCommandRepo : Repo<DiscordBotCommandModel>, IDiscordBotCommandRepo
{
    private readonly DiscordBotContext _context;

    public DiscordBotCommandRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task AddOrUpdateAsync(DiscordBotModel bot, string fullCommandName, CancellationToken cancellationToken = default)
    {
        var command = await _context.DiscordBotCommands.FirstOrAddAsync(x => x.CommandName == fullCommandName,
            () => new DiscordBotCommandModel { CommandName = fullCommandName, Bot = bot }, cancellationToken: cancellationToken);

        command.LastUsedOn = DateTime.UtcNow;
        command.Used++;
    }

    public async Task AddOrUpdateAsync(Guid botGuid, string fullCommandName, CancellationToken cancellationToken = default)
    {
        var bot = await new DiscordBotRepo(_context).GetOrAddAsync(botGuid, cancellationToken);
        await AddOrUpdateAsync(bot, fullCommandName, cancellationToken);
    }
}
