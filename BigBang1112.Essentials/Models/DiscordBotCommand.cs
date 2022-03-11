using BigBang1112.Services;
using Discord;
using Discord.WebSocket;

namespace BigBang1112.Models;

public abstract class DiscordBotCommand
{
    private readonly DiscordBotService _discordBotService;

    public DiscordBotCommand(DiscordBotService discordBotService)
    {
        _discordBotService = discordBotService ?? throw new ArgumentNullException(nameof(discordBotService));
    }

    public abstract Task<DiscordBotMessage> ExecuteAsync(SocketSlashCommand slashCommand);
    public abstract Task<DiscordBotMessage?> SelectMenuAsync(SocketMessageComponent messageComponent);

    public string CreateCustomId(string? additional = null)
    {
        if (_discordBotService.CommandAttributes.TryGetValue(GetType(), out var commandAttribute))
        {
            var customId = string.IsNullOrWhiteSpace(additional)
                ? commandAttribute.Name
                : $"{commandAttribute.Name}-{additional}";

            return customId.ToLower().Replace(' ', '_');
        }

        throw new Exception();
    }
}
