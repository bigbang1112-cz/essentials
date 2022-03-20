using Discord;

namespace BigBang1112.DiscordBot.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DiscordBotCommandOptionAttribute : Attribute
{
    public string Name { get; }
    public ApplicationCommandOptionType Type { get; }
    public string Description { get; }

    public bool IsRequired { get; set; }
    public bool IsDefault { get; set; }
    public double MinValue { get; set; } = double.MinValue;
    public double MaxValue { get; set; } = double.MaxValue;

    public DiscordBotCommandOptionAttribute(string name, ApplicationCommandOptionType type, string description)
    {
        Name = name;
        Type = type;
        Description = description;
    }
}
