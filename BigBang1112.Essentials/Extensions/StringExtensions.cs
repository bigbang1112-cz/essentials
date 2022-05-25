namespace BigBang1112.Extensions;

public static class StringExtensions
{
    private static bool EscapableChar(char c)
    {
        return c == '*' || c == '_' || c == '|';
    }

    public static string EscapeDiscord(this string str)
    {
        var counter = 0;

        foreach (char c in str)
        {
            if (EscapableChar(c))
            {
                counter++;
            }
        }

        if (counter == 0)
        {
            return str;
        }

        Span<char> newChars = stackalloc char[str.Length + counter];

        var counter2 = 0;

        // insert backward slashes before each char in discordEscapeChars

        for (var i = 0; i < str.Length; i++)
        {
            var c = str[i];

            if (EscapableChar(c))
            {
                newChars[i + counter2] = '\\';
                counter2++;
            }

            newChars[i + counter2] = c;
        }

        return newChars.ToString();
    }
}
