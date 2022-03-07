namespace BigBang1112.Attributes.DiscordBot;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordBotCommandOptionAttribute : Attribute
{
    public string Name { get; }

    public DiscordBotCommandOptionAttribute(string name)
    {
        Name = name;
    }
}
