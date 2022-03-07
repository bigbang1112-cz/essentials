namespace BigBang1112.Attributes.DiscordBot;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordBotCommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public DiscordBotCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
