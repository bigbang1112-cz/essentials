namespace BigBang1112.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnix(this DateTime dateTime)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)).ToUnixTimeSeconds();
    }
}
