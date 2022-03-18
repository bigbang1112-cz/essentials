﻿using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Models;

public class Deferer
{
    private readonly SocketSlashCommand slashCommand;

    public bool IsDeferred { get; private set; }

    public Deferer(SocketSlashCommand slashCommand)
    {
        this.slashCommand = slashCommand;
    }

    public async Task DeferAsync(bool ephemeral = false)
    {
        await slashCommand.DeferAsync(ephemeral);
        IsDeferred = true;
    }
}