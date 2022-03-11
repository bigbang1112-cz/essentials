﻿namespace BigBang1112.Attributes.DiscordBot;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordBotSubCommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public DiscordBotSubCommandAttribute(string name, string description = "Does nothing at the moment.")
    {
        Name = name;
        Description = description;
    }
}
