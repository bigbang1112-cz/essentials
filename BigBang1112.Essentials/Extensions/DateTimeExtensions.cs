using Discord;

namespace BigBang1112.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnix(this DateTime dateTime)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)).ToUnixTimeSeconds();
    }

    public static TimestampTag ToTimestampTag(this DateTime dateTime, TimestampTagStyles style = TimestampTagStyles.ShortDateTime)
    {
        return TimestampTag.FromDateTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), style);
    }
}
