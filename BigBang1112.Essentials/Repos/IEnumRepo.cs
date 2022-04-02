using BigBang1112.Models.Db;

namespace BigBang1112.Repos;

public interface IEnumRepo<TEntity, TEnum>
    where TEntity : DbModel
    where TEnum : Enum
{
    TEntity Get(TEnum value);
    Task<TEntity> GetAsync(TEnum value, CancellationToken cancellationToken = default);
}
