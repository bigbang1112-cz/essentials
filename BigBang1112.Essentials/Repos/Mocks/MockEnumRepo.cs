using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Mocks;

public abstract class MockEnumRepo<TEntity, TEnum> : MockRepo<TEntity>, IEnumRepo<TEntity, TEnum>
    where TEntity : DbModel
    where TEnum : Enum
{
    public TEntity Get(TEnum value)
    {
        return Entities.Single(x => x.Id == (int)(object)value);
    }

    public async Task<TEntity> GetAsync(TEnum value, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Get(value));
    }
}
