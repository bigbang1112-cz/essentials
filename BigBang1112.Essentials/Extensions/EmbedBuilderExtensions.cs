using Discord;

namespace BigBang1112.Extensions;

public static class EmbedBuilderExtensions
{
    public static EmbedBuilder WithBotFooter(this EmbedBuilder builder, string text)
    {
        return builder.WithFooter(text, "https://wr.bigbang1112.cz/favicon/favicon-32x32.png");
    }
}
