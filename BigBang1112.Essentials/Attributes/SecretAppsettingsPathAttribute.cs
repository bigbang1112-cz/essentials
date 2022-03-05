namespace BigBang1112.Attributes;

public class SecretAppsettingsPathAttribute : Attribute
{
    public string Path { get; }

    public SecretAppsettingsPathAttribute(string path)
    {
        Path = path;
    }
}
