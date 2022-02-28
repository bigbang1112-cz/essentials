using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace BigBang1112.Extensions;

public static class HeaderDictionaryExtensions
{
    public static void AddLastModified(this IHeaderDictionary headerDictionary, DateTimeOffset date)
    {
        headerDictionary.Add("Last-Modified",
            date.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'",
            CultureInfo.InvariantCulture.DateTimeFormat));
    }
}
