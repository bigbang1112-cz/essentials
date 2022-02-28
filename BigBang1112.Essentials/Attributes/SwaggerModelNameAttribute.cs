namespace BigBang1112.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SwaggerModelNameAttribute : Attribute
{
    public string Name { get; init; }

    public SwaggerModelNameAttribute(string name)
    {
        Name = name;
    }
}
