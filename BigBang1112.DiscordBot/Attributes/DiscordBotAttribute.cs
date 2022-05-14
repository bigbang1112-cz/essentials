namespace BigBang1112.DiscordBot.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordBotAttribute : Attribute
{
    public Guid Guid { get; }
    public string Name { get; }
    public string Version { get; }
    public string? Punchline { get; set; }
    public string? Description { get; set; }
    public string? GitRepoUrl { get; set; }

    public DiscordBotAttribute(string guid, string name, string version)
    {
        Guid = new Guid(guid);
        Name = name;
        Version = version;
    }
}
