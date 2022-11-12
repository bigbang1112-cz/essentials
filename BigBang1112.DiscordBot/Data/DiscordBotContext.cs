using BigBang1112.DiscordBot.Models;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.UniReminder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace BigBang1112.DiscordBot.Data;

public class DiscordBotContext : DbContext
{
    private readonly IEncryptionProvider encryption;

    public DbSet<DiscordBotModel> DiscordBots { get; set; } = default!;
    public DbSet<DiscordBotGuildModel> DiscordBotGuilds { get; set; } = default!;
    public DbSet<DiscordBotChannelModel> DiscordBotChannels { get; set; } = default!;
    public DbSet<DiscordBotJoinedGuildModel> DiscordBotJoinedGuilds { get; set; } = default!;
    public DbSet<DiscordBotCommandVisibilityModel> DiscordBotCommandVisibilities { get; set; } = default!;
    public DbSet<DiscordBotCommandModel> DiscordBotCommands { get; set; } = default!;
    public DbSet<MemeModel> Memes { get; set; } = default!;
    public DbSet<DiscordUserModel> DiscordUsers { get; set; } = default!;
    public DbSet<FeedbackModel> Feedbacks { get; set; } = default!;
    public DbSet<PingMessageModel> PingMessages { get; set; } = default!;
    public DbSet<ReportChannelModel> ReportChannels { get; set; } = default!;
    public DbSet<ReportChannelMessageModel> ReportChannelMessages { get; set; } = default!;
    public DbSet<PredmetModel> Predmety { get; set; } = default!;

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

        modelBuilder.Entity<ReportChannelModel>()
            .Property(e => e.ThreadOptions)
            .HasColumnType("text")
            .HasConversion(
                x => JsonSerializer.Serialize(x, AutoThreadOptions.JsonSerializerOptions),
                x => JsonSerializer.Deserialize<AutoThreadOptions>(x, AutoThreadOptions.JsonSerializerOptions));
    }
}
