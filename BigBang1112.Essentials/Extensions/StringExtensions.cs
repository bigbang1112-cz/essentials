namespace BigBang1112.Extensions;

public static class StringExtensions
{
    public static string EscapeDiscord(this string str)
    {
        return str.Replace("_", "\\_").Replace("*", "\\*").Replace("|", "\\|");
    }
}
