using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models;
using Discord;
using Discord.WebSocket;
using BigBang1112.DiscordBot.Data;

namespace BigBang1112.DiscordBot.Commands;

public partial class VisibilityCommand
{
    [DiscordBotSubCommand("channel", "Gets or sets the channel visibility of command executions for this bot.")]
    public class Channel : DiscordBotCommand
    {
        private readonly IDiscordBotRepo _repo;

        [DiscordBotCommandOption("set", ApplicationCommandOptionType.Boolean, "If True, major command executions will be visible to everyone in this channel [ManageChannels].")]
        public bool? Set { get; set; }

        [DiscordBotCommandOption("other", ApplicationCommandOptionType.Channel, "Specify other channel to apply/see the visibility to/of.")]
        public SocketChannel? OtherChannel { get; set; }

        public Channel(DiscordBotService discordBotService, IDiscordBotRepo repo) : base(discordBotService)
        {
            _repo = repo;
        }

        public override async Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand)
        {
            if (OtherChannel is not SocketTextChannel textChannel)
            {
                if (OtherChannel is not null)
                {
                    return RespondWithDescriptionEmbed("The specified channel is not a guild text channel (categories will be supported later).");
                }

                if (slashCommand.Channel is not SocketTextChannel guildTextChannel)
                {
                    return RespondWithDescriptionEmbed("The specified channel is not a guild text channel.");
                }

                textChannel = guildTextChannel;
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
