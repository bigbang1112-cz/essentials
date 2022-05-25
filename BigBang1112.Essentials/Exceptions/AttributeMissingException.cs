namespace BigBang1112.Exceptions;

public class AttributeMissingException : Exception
{
    public AttributeMissingException() { }
    public AttributeMissingException(string message) : base(message) { }
    public AttributeMissingException(string message, Exception inner) : base(message, inner) { }
    protected AttributeMissingException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
