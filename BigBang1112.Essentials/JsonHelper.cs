using BigBang1112.Exceptions;
using System.Text.Json;

namespace BigBang1112;

public static class JsonHelper
{
    public static TValue Deserialize<TValue>(string json, JsonSerializerOptions? options = null)
    {
        var value = JsonSerializer.Deserialize<TValue>(json, options);

        if (value is null)
        {
            throw new ThisShouldNotHappenException();
        }

        return value;
    }

    public static async ValueTask<TValue> DeserializeAsync<TValue>(Stream utf8Json,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var value = await JsonSerializer.DeserializeAsync<TValue>(utf8Json, options, cancellationToken);

        if (value is null)
        {
            throw new ThisShouldNotHappenException();
        }

        return value;
    }
}
