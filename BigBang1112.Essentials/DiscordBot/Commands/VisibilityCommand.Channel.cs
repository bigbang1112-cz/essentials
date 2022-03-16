using BigBang1112.Attributes.DiscordBot;
using BigBang1112.Data;
using BigBang1112.Models.DiscordBot;
using BigBang1112.Services;
using Discord;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Commands;

public partial class VisibilityCommand
{
    [DiscordBotSubCommand("channel", "Gets or sets the channel visibility of command executions for this bot.")]
    public class Channel : DiscordBotCommand
    {
        private readonly IAccountsRepo _repo;

        [DiscordBotCommandOption("set", ApplicationCommandOptionType.Boolean, "Set to True or False.")]
        public bool? Set { get; set; }

        [DiscordBotCommandOption("otherchannel", ApplicationCommandOptionType.Channel, "Specify other channel to apply/see the visibility to/of.")]
        public SocketChannel? SpecificChannel { get; set; }

        public Channel(DiscordBotService discordBotService, IAccountsRepo repo) : base(discordBotService)
        {
            _repo = repo;
        }

        public override async Task<DiscordBotMessage> ExecuteAsync(SocketSlashCommand slashCommand)
        {
            if (SpecificChannel is not SocketTextChannel textChannel)
            {
                if (SpecificChannel is not null)
                {
                    return RespondWithDescriptionEmbed("The specified channel is not a text channel (categories will be supported later).");
                }

                textChannel = slashCommand.Channel as SocketTextChannel ?? throw new Exception();
            }

            if (Set is null)
            {
                return await GetVisibilityAsync(textChannel)
                    ? RespondWithDescriptionEmbed($"In <#{textChannel.Id}>, most of the commmand executions **are visible**.")
                    : RespondWithDescriptionEmbed($"In <#{textChannel.Id}>, most of the commmand executions are visible **based on the global setting**.");
            }

            if (slashCommand.User is not SocketGuildUser guildUser)
            {
                throw new Exception("This user is not a guild user");
            }

            if (!guildUser.GuildPermissions.ManageChannels)
            {
                return RespondWithDescriptionEmbed($"You don't have permissions to set the visibility of command executions in <#{textChannel.Id}>.");
            }

            try
            {
                return await SetVisibilityAsync(textChannel, Set.Value)
                    ? RespondWithDescriptionEmbed($"In <#{textChannel.Id}>, most of the commmand executions are now **visible**.")
                    : RespondWithDescriptionEmbed($"In <#{textChannel.Id}>, most of the commmand executions are now visible **based on the global setting**.");
            }
            catch (Exception)
            {
                return RespondWithDescriptionEmbed($"Cannot set command execution visibility for <#{textChannel.Id}> due to exception.");
            }
        }

        private async Task<bool> GetVisibilityAsync(SocketTextChannel textChannel)
        {
            var discordBotGuid = GetDiscordBotGuid();

            if (discordBotGuid is null)
            {
                return false;
            }

            var commandVisibility = await _repo.GetDiscordBotCommandVisibilityAsync(discordBotGuid.Value, textChannel);

            if (commandVisibility is null || !commandVisibility.Visibility)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> SetVisibilityAsync(SocketTextChannel textChannel, bool set)
        {
            var discordBotGuid = GetDiscordBotGuid();

            if (discordBotGuid is null)
            {
                throw new Exception("Missing discord bot guid");
            }

            await _repo.AddOrUpdateDiscordBotCommandVisibilityAsync(discordBotGuid.Value, textChannel, set);

            await _repo.SaveAsync();

            return set; 
        }

        private static DiscordBotMessage RespondWithDescriptionEmbed(string description)
        {
            var embed = new EmbedBuilder()
                .WithDescription(description)
                .Build();

            return new DiscordBotMessage(embed, ephemeral: true);
        }
    }
}
