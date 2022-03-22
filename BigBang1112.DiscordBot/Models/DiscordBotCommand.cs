using Discord;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Models;

public abstract class DiscordBotCommand
{
    private readonly DiscordBotService _discordBotService;

    public DiscordBotCommand(DiscordBotService discordBotService)
    {
        _discordBotService = discordBotService ?? throw new ArgumentNullException(nameof(discordBotService));
    }

    public virtual Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand)
    {
        return Task.FromResult(new DiscordBotMessage("Command not implemented.", ephemeral: true));
    }

    public virtual async Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand, Deferer deferer)
    {
        return await ExecuteAsync(slashCommand);
    }

    public virtual Modal? ExecuteModal(SocketSlashCommand slashCommand)
    {
        return null;
    }

    public virtual Task<DiscordBotMessage?> SelectMenuAsync(SocketMessageComponent messageComponent, Deferer deferer)
    {
        return Task.FromResult(default(DiscordBotMessage));
    }

    public virtual Task<DiscordBotMessage?> ExecuteButtonAsync(SocketMessageComponent messageComponent, Deferer deferer)
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
