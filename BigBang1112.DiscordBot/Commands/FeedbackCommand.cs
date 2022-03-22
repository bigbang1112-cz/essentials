using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models;
using Discord;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Commands;

[DiscordBotCommand("feedback", "Share some feedback!")]
public class FeedbackCommand : DiscordBotCommand
{
    private readonly IDiscordBotRepo _discordBotRepo;

    public FeedbackCommand(DiscordBotService discordBotService, IDiscordBotRepo discordBotRepo) : base(discordBotService)
    {
        _discordBotRepo = discordBotRepo;
    }

    public override async Task<Modal?> ExecuteModalAsync(SocketSlashCommand slashCommand)
    {
        var guid = GetDiscordBotGuid() ?? throw new Exception();

        var user = await _discordBotRepo.AddOrUpdateDiscordUserAsync(guid, slashCommand.User);

        if (user.IsBlocked)
        {
            return null;
        }

        return new ModalBuilder()
            .WithCustomId("feedback-modal")
            .WithTitle("Share feedback")
            .AddTextInput("Tell me something...", "feedback-text", TextInputStyle.Paragraph, " ...bugs, feature suggestions, review...", required: true)
            .Build();
    }

    public override Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand)
    {
        var embed = new EmbedBuilder { Title = "You're blocked from using /feedback." }.Build();

        return Task.FromResult(new DiscordBotMessage(embed, ephemeral: true));
    }
}
