namespace BigBang1112.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereWith<T>(this IEnumerable<T> enumerable,
        IEnumerable<T> anotherEnumerable,
        Func<T, object> whenMatching,
        Func<T, T, bool> andApplies)
    {
        foreach (var item1 in enumerable)
        {
            var item2 = anotherEnumerable.FirstOrDefault(item2 => whenMatching(item1) == whenMatching(item2));

            if (item2 is null)
            {
                continue;
            }

            if (andApplies(item1, item2))
            {
                yield return item1;
            }
        }
    }
}
