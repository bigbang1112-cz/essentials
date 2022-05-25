using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class DiscordUserRepo : Repo<DiscordUserModel>, IDiscordUserRepo
{
    private readonly DiscordBotContext _context;

    public DiscordUserRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<DiscordUserModel> AddOrUpdateAsync(DiscordBotModel bot, SocketUser user, CancellationToken cancellationToken = default)
    {
        var userModel = await _context.DiscordUsers
            .FirstOrDefaultAsync(x => x.Bot == bot && x.Snowflake == user.Id, cancellationToken);

        if (userModel is null)
        {
            userModel = new DiscordUserModel
            {
                Snowflake = user.Id,
                Name = user.Username,
                Discriminator = user.DiscriminatorValue,
                Bot = bot,
                FirstInteractionOn = DateTime.UtcNow,
                LastInteractionOn = DateTime.UtcNow,
            };

            await _context.DiscordUsers.AddAsync(userModel, cancellationToken);

            return userModel;
        }

        userModel.Name = user.Username;
        userModel.Discriminator = user.DiscriminatorValue;
        userModel.LastInteractionOn = DateTime.UtcNow;
        userModel.Interactions++;

        return userModel;
    }

    public async Task<DiscordUserModel> AddOrUpdateAsync(Guid botGuid, SocketUser user, CancellationToken cancellationToken = default)
    {
        var bot = await new DiscordBotRepo(_context).GetOrAddAsync(botGuid, cancellationToken);
        return await AddOrUpdateAsync(bot, user, cancellationToken);
    }
}
