using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class ReportChannelMessageRepo : Repo<ReportChannelMessageModel>, IReportChannelMessageRepo
{
    private readonly DiscordBotContext _context;

    public ReportChannelMessageRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReportChannelMessageModel>> GetAllByReportGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await _context.ReportChannelMessages
            .Where(x => x.ReportGuid == guid)
            .ToListAsync(cancellationToken);
    }
}
