using System.Reflection;
using BigBang1112.Exceptions;
using BigBang1112.Models.Db;

namespace BigBang1112;

public static class EnumData
{
    public static IEnumerable<TEntity> Create<TEntity, TEnum, TAttribute>(Func<TAttribute, TEntity> seed)
        where TEntity : DbModel where TEnum : struct, Enum where TAttribute : Attribute
    {
        return Enum.GetValues<TEnum>().Select(game =>
        {
            var fieldInfo = typeof(TEnum).GetField(game.ToString()) ?? throw new ThisShouldNotHappenException("Field doesn't exist even though it should");
            var att = fieldInfo.GetCustomAttribute<TAttribute>() ?? throw new AttributeMissingException("GameAttribute missing");
            var model = seed(att);
            model.Id = (int)(object)game;
            return model;
        });
    }
}
