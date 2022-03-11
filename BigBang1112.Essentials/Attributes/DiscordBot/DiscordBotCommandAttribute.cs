﻿namespace BigBang1112.Attributes.DiscordBot;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordBotCommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public DiscordBotCommandAttribute(string name, string description = "Does nothing at the moment.")
    {
        Name = name;
        Description = description;
    }
}
