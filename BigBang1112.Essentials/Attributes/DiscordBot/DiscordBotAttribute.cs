namespace BigBang1112.Attributes.DiscordBot;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordBotAttribute : Attribute
{
    public Guid Guid { get; }
    public string Name { get; }

    public DiscordBotAttribute(string guid, string name)
    {
        Guid = new Guid(guid);
        Name = name;
    }
}
