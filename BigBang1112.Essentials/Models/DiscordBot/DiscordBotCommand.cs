using BigBang1112.Attributes.DiscordBot;
using BigBang1112.Services;
using Discord;
using Discord.WebSocket;

namespace BigBang1112.Models.DiscordBot;

public abstract class DiscordBotCommand
{
    private readonly DiscordBotService _discordBotService;

    public DiscordBotCommand(DiscordBotService discordBotService)
    {
        _discordBotService = discordBotService ?? throw new ArgumentNullException(nameof(discordBotService));
    }

    public abstract Task<DiscordBotMessage> ExecuteAsync(SocketSlashCommand slashCommand);

    public virtual Task<DiscordBotMessage?> SelectMenuAsync(SocketMessageComponent messageComponent)
    {
        return Task.FromResult(default(DiscordBotMessage));
    }

    protected string CreateCustomId(string? additional = null)
    {
        var type = GetType();
        var name = _discordBotService.Commands
            .FirstOrDefault(x => x.Value == type)
            .Key.Replace(' ', '_');
        var customId = string.IsNullOrWhiteSpace(additional) ? name : $"{name}-{additional}";

        return customId.ToLower().Replace(' ', '_');
    }

    protected Guid? GetDiscordBotGuid()
    {
        return _discordBotService.GetGuid();
    }
}
