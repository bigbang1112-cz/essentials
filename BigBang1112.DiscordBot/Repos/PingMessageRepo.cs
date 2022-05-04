﻿using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class PingMessageRepo : Repo<PingMessageModel>, IPingMessageRepo
{
    public PingMessageRepo(DbContext context) : base(context)
    {
    }
}
