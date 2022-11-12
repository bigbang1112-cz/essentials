using BigBang1112.Data;
using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using BigBang1112.UniReminder.Models;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class PredmetRepo : Repo<PredmetModel>, IPredmetRepo
{
    private readonly DiscordBotContext _context;

    public PredmetRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> GetAllNazvyLikeAsync(string value, int limit = DiscordConsts.OptionLimit, CancellationToken cancellationToken = default)
    {
        return await _context.Predmety.Select(x => x.Nazev)
            .Where(x => x.Contains(value))
            .Distinct()
            .OrderByDescending(x => x.StartsWith(value))
            .ThenBy(x => x)
            .Take(limit)
            .Cacheable()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PredmetModel>> GetAllLikePracovisteAndKodAsync(string pracoviste, string? kod, int limit = DiscordConsts.OptionLimit, CancellationToken cancellationToken = default)
    {
        if (kod is null)
        {
            return await _context.Predmety
                .Where(x => x.Pracoviste.Contains(pracoviste) || x.Predmet.Contains(pracoviste))
                .Distinct()
                .OrderByDescending(x => x.Pracoviste.StartsWith(pracoviste))
                .ThenBy(x => x.Pracoviste)
                .ThenBy(x => x.Predmet)
                .Take(limit)
                .Cacheable()
                .ToListAsync(cancellationToken);
        }

        return await _context.Predmety
            .Where(x => x.Pracoviste.Contains(pracoviste) || x.Predmet.Contains(kod))
            .Distinct()
            .OrderByDescending(x => x.Pracoviste.StartsWith(pracoviste) && x.Predmet.StartsWith(kod))
            .ThenBy(x => x.Pracoviste.StartsWith(pracoviste))
            .ThenBy(x => x.Pracoviste)
            .ThenBy(x => x.Predmet)
            .Take(limit)
            .Cacheable()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PredmetModel>> GetAllByNameAsync(string nazev, int limit = 25, CancellationToken cancellationToken = default)
    {
        return await _context.Predmety
            .Where(x => string.Equals(x.Nazev, nazev))
            .Distinct()
            .OrderByDescending(x => x.Pracoviste)
            .ThenBy(x => x.Predmet)
            .Take(limit)
            .Cacheable()
            .ToListAsync(cancellationToken);
    }

    public async Task<PredmetModel?> GetByPracovisteAndPredmetAsync(string pracoviste, string predmet, CancellationToken cancellationToken = default)
    {
        return await _context.Predmety.FirstOrDefaultAsync(
            x => string.Equals(x.Pracoviste, pracoviste) && string.Equals(x.Predmet, predmet), cancellationToken);
    }

    public async Task<PredmetModel?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await _context.Predmety.FirstOrDefaultAsync(x => x.Guid == guid, cancellationToken);
    }
}
