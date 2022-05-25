using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public interface IDiscordAuthRepo : IRepo<DiscordAuthModel>
{
    Task<DiscordAuthModel> GetOrAddAsync(ulong snowflake, CancellationToken cancellationToken = default);
}
