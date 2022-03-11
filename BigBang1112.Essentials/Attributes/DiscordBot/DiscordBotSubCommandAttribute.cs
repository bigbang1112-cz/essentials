namespace BigBang1112.Attributes.DiscordBot;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordBotSubCommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public DiscordBotSubCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
