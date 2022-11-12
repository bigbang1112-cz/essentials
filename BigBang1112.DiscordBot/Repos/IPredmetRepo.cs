using BigBang1112.Data;
using BigBang1112.UniReminder.Models;

namespace BigBang1112.DiscordBot.Repos;

public interface IPredmetRepo
{
    Task<IEnumerable<PredmetModel>> GetAllByNameAsync(string nazev, int limit = DiscordConsts.OptionLimit, CancellationToken cancellationToken = default);
    Task<IEnumerable<PredmetModel>> GetAllLikePracovisteAndKodAsync(string pracoviste, string? kod, int limit = DiscordConsts.OptionLimit, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetAllNazvyLikeAsync(string value, int limit = DiscordConsts.OptionLimit, CancellationToken cancellationToken = default);
    Task<PredmetModel?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<PredmetModel?> GetByPracovisteAndPredmetAsync(string pracoviste, string predmet, CancellationToken cancellationToken = default);
}