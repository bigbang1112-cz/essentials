using Discord;

namespace BigBang1112.Extensions;

public static class DateTimeOffsetExtensions
{
    public static TimestampTag ToTimestampTag(this DateTimeOffset dateTime, TimestampTagStyles style = TimestampTagStyles.ShortDateTime)
    {
        return TimestampTag.FromDateTime(dateTime.UtcDateTime, style);
    }
}
