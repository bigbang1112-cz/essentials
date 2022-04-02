using BigBang1112.Exceptions;
using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace BigBang1112.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static DataBuilder<TEntity> HasEnumData<TEntity, TEnum, TAttribute>(this EntityTypeBuilder<TEntity> entityTypeBuilder,
        Func<TAttribute, TEntity> seed)
        where TEntity : DbModel where TEnum : struct, Enum where TAttribute : Attribute
    {
        return entityTypeBuilder.HasData(Enum.GetValues<TEnum>().Select(game =>
        {
            var fieldInfo = typeof(TEnum).GetField(game.ToString()) ?? throw new ThisShouldNotHappenException("Field doesn't exist even though it should");
            var att = fieldInfo.GetCustomAttribute<TAttribute>() ?? throw new AttributeMissingException("GameAttribute missing");
            var model = seed(att);
            model.Id = (int)(object)game;
            return model;
        }));
    }
}
