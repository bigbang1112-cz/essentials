namespace BigBang1112.Attributes.DiscordBot;

[AttributeUsage(AttributeTargets.Property)]
public class DiscordBotCommandOptionAutocompleteAttribute : Attribute
{
    public string MethodName { get; }

    public DiscordBotCommandOptionAutocompleteAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
