using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Models;

public class Deferer
{
    private readonly SocketInteraction slashCommand;

    public bool IsDeferred { get; private set; }
    public bool Ephemeral { get; init; }

    public Deferer(SocketInteraction slashCommand, bool ephemeral)
    {
        this.slashCommand = slashCommand;

        Ephemeral = ephemeral;
    }

    public async Task DeferAsync(bool ephemeral = false)
    {
        await slashCommand.DeferAsync(ephemeral ? ephemeral : Ephemeral);
        IsDeferred = true;
    }
}
