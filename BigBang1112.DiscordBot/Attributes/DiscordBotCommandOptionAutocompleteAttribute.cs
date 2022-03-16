namespace BigBang1112.DiscordBot.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DiscordBotCommandOptionAutocompleteAttribute : Attribute
{
    public string MethodName { get; }

    public DiscordBotCommandOptionAutocompleteAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
