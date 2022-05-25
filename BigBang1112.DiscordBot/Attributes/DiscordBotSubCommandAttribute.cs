namespace BigBang1112.DiscordBot.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordBotSubCommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public DiscordBotSubCommandAttribute(string name, string description = "Does nothing at the moment.")
    {
        if (description.Length > 100)
        {
            throw new Exception($"Subcommand '{name}' description (\"{description}\") is over 100 characters.");
        }

        Name = name;
        Description = description;
    }
}
