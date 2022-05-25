using BigBang1112.Exceptions;
using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.Repos;

public class EnumRepo<TEntity, TEnum> : Repo<TEntity>, IEnumRepo<TEntity, TEnum>
    where TEntity : DbModel
    where TEnum : Enum
{
    public EnumRepo(DbContext context) : base(context)
    {

    }

    public TEntity Get(TEnum value)
    {
        return GetById((int)(object)value) ?? throw new ThisShouldNotHappenException("Game not found, even though it should exist");
    }

    public async Task<TEntity> GetAsync(TEnum value, CancellationToken cancellationToken = default)
    {
        return (await GetByIdAsync((int)(object)value, cancellationToken)) ?? throw new ThisShouldNotHappenException("Game not found, even though it should exist");
    }
}
