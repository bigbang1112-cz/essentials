﻿using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models;
using Discord;
using Discord.WebSocket;
using BigBang1112.DiscordBot.Data;

namespace BigBang1112.DiscordBot.Commands;

public partial class VisibilityCommand
{
    [DiscordBotSubCommand("global", "Gets or sets the global visibility of command executions for this bot.")]
    public class Global : DiscordBotCommand
    {
        private readonly IDiscordBotUnitOfWork _discordBotUnitOfWork;

        [DiscordBotCommandOption("set", ApplicationCommandOptionType.Boolean, "If True, major command executions will be visible to everyone [ManageGuild].")]
        public bool? Set { get; set; }

        public Global(DiscordBotService discordBotService, IDiscordBotUnitOfWork discordBotUnitOfWork) : base(discordBotService)
        {
            _discordBotUnitOfWork = discordBotUnitOfWork;
        }

        public override async Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand)
        {
            if (slashCommand.Channel is not SocketTextChannel textChannel)
            { 
                return RespondWithDescriptionEmbed("You're not executing the command in a guild text channel.");
            }

            if (Set is null)
            {
                return await GetVisibilityAsync(textChannel)
                    ? RespondWithDescriptionEmbed($"Most of the commmand executions **are visible**.")
                    : RespondWithDescriptionEmbed($"All of the commmand executions are **invisible**.");
            }

            if (slashCommand.User is not SocketGuildUser guildUser)
            {
                throw new Exception("This user is not a guild user");
            }
            
            if (!guildUser.GuildPermissions.ManageGuild)
            {
                return RespondWithDescriptionEmbed($"You don't have permissions to set the global visibility of command executions.");
            }

            try
            {
                return await SetVisibilityAsync(textChannel, Set.Value)
                    ? RespondWithDescriptionEmbed($"Most of the commmand executions **are now visible**.")
                    : RespondWithDescriptionEmbed($"All of the commmand executions are now **invisible**.");
            }
            catch (Exception)
            {
                return RespondWithDescriptionEmbed($"Cannot set command execution visibility due to exception.");
            }
        }

        private async Task<bool> GetVisibilityAsync(SocketTextChannel textChannel)
        {
            var discordBotGuid = GetDiscordBotGuid();

            if (discordBotGuid is null)
            {
                return false;
            }

            var joinedGuild = await _discordBotUnitOfWork.DiscordBotJoinedGuilds
                .GetByBotAndTextChannelAsync(discordBotGuid.Value, textChannel);

            if (joinedGuild is null || !joinedGuild.CommandVisibility)
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

            var joinedGuild = await _discordBotUnitOfWork.DiscordBotJoinedGuilds
                .GetOrAddAsync(discordBotGuid.Value, textChannel.Guild);

            joinedGuild.CommandVisibility = set;

            await _discordBotUnitOfWork.SaveAsync();

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
