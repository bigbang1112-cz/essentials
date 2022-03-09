using Discord;

namespace BigBang1112.Models;

public class DiscordBotMessage
{
    public string? Message { get; init; }
    public Embed[]? Embeds { get; init; }
    public MessageComponent? Component { get; init; }
    public bool Ephemeral { get; init; }

    public DiscordBotMessage(string? message = null, Embed[]? embeds = null, MessageComponent? component = null, bool ephemeral = false)
    {
        Message = message;
        Embeds = embeds;
        Component = component;
        Ephemeral = ephemeral;
    }

    public DiscordBotMessage(Embed embed, MessageComponent? component = null, bool ephemeral = false)
        : this(message: null, new Embed[] { embed }, component, ephemeral)
    {

    }
}
