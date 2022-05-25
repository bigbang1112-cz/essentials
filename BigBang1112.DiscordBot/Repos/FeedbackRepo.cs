using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class FeedbackRepo : Repo<FeedbackModel>, IFeedbackRepo
{
    public FeedbackRepo(DbContext context) : base(context)
    {
    }
}
