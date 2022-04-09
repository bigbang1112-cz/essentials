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
        return entityTypeBuilder.HasData(EnumData.Create<TEntity, TEnum, TAttribute>(seed));
    }
}
