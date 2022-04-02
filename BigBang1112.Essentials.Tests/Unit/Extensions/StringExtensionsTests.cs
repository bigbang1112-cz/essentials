using BigBang1112.Extensions;
using Xunit;

namespace BigBang1112.Tests.Unit.Extensions;

public class StringExtensionsTests
{
    // test EscapeDiscord
    [Fact]
    public void EscapeDiscord_Formatted_ReturnsEscapedString()
    {
        // arrange
        var input = "**This** *is* _a test_ ||string||";
        var expected = "\\*\\*This\\*\\* \\*is\\* \\_a test\\_ \\|\\|string\\|\\|";

        // act
        var actual = input.EscapeDiscord();

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void EscapeDiscord_NotFormatted_ReturnsSameString()
    {
        // arrange
        var input = "This is a test string";
        var expected = input;

        // act
        var actual = input.EscapeDiscord();

        // assert
        Assert.Equal(expected, actual);
    }
}
