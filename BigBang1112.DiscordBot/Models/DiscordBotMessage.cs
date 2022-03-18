using Discord;

namespace BigBang1112.DiscordBot.Models;

public class DiscordBotMessage
{
    public string? Message { get; init; }
    public Embed[]? Embeds { get; init; }
    public MessageComponent? Component { get; init; }
    public bool Ephemeral { get; init; }
    public bool AlwaysPostAsNewMessage { get; init; }
    public FileAttachment? Attachment { get; init; }

    public DiscordBotMessage(string? message = null, Embed[]? embeds = null, MessageComponent? component = null, bool ephemeral = false, bool alwaysPostAsNewMessage = false, FileAttachment? attachment = null)
    {
        Message = message;
        Embeds = embeds;
        Component = component;
        Ephemeral = ephemeral;
        AlwaysPostAsNewMessage = alwaysPostAsNewMessage;
        Attachment = attachment;
    }

    public DiscordBotMessage(Embed embed, MessageComponent? component = null, bool ephemeral = false, bool alwaysPostAsNewMessage = false, FileAttachment? attachment = null)
        : this(message: null, new Embed[] { embed }, component, ephemeral, alwaysPostAsNewMessage, attachment)
    {

    }
}
