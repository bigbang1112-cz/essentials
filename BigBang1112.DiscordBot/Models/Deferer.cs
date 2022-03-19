using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Models;

public class Deferer
{
    private readonly SocketInteraction slashCommand;

    public bool IsDeferred { get; private set; }

    public Deferer(SocketInteraction slashCommand)
    {
        this.slashCommand = slashCommand;
    }

    public async Task DeferAsync(bool ephemeral = false)
    {
        await slashCommand.DeferAsync(ephemeral);
        IsDeferred = true;
    }
}
