using BigBang1112.DiscordBot.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BigBang1112.DiscordBot.Data;

public class DiscordBotContext : DbContext
{
    private readonly IEncryptionProvider encryption;

    public DbSet<DiscordBotModel> DiscordBots { get; set; } = default!;
    public DbSet<DiscordBotGuildModel> DiscordBotGuilds { get; set; } = default!;
    public DbSet<DiscordBotChannelModel> DiscordBotChannels { get; set; } = default!;
    public DbSet<DiscordBotJoinedGuildModel> DiscordBotJoinedGuilds { get; set; } = default!;
    public DbSet<DiscordBotCommandVisibilityModel> DiscordBotCommandVisibilities { get; set; } = default!;
    public DbSet<MemeModel> Memes { get; set; } = default!;
    public DbSet<DiscordUserModel> DiscordUsers { get; set; } = default!;
    public DbSet<FeedbackModel> Feedbacks { get; set; } = default!;
    public DbSet<PingMessageModel> PingMessages { get; set; } = default!;

    public DiscordBotContext(DbContextOptions<DiscordBotContext> options, IConfiguration config) : base(options)
    {
        var key = Encoding.ASCII.GetBytes(config["DiscordBotDbEncryptionKey"]);
        var iv = Encoding.ASCII.GetBytes(config["DiscordBotDbEncryptionIV"]);

        encryption = new AesProvider(key, iv);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseEncryption(encryption);
    }
}