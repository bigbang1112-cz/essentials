using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BigBang1112.Data;

public class AccountsContext : DbContext
{
    private readonly IEncryptionProvider encryption;

    public DbSet<AccountModel> Accounts { get; set; } = default!;
    public DbSet<ManiaPlanetAuthModel> ManiaPlanetAuth { get; set; } = default!;
    public DbSet<TrackmaniaAuthModel> TrackmaniaAuth { get; set; } = default!;
    public DbSet<DiscordAuthModel> DiscordAuth { get; set; } = default!;
    public DbSet<GitHubAuthModel> GitHubAuth { get; set; } = default!;
    public DbSet<TwitterAuthModel> TwitterAuth { get; set; } = default!;
    public DbSet<ManiaPlanetLbManialinkModel> LbManialink { get; set; } = default!;
    public DbSet<AdminModel> Admins { get; set; } = default!;
    public DbSet<ZoneModel> Zones { get; set; } = default!;

    public AccountsContext(DbContextOptions<AccountsContext> options, IConfiguration config) : base(options)
    {
        var key = Encoding.ASCII.GetBytes(config["AccountsDbEncryptionKey"]);
        var iv = Encoding.ASCII.GetBytes(config["AccountsDbEncryptionIV"]);

        encryption = new AesProvider(key, iv);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseEncryption(encryption);

        modelBuilder.Entity<ManiaPlanetAuthModel>()
            .HasOne(x => x.LbManialink)
            .WithOne(x => x.Auth)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AccountModel>()
            .HasOne(x => x.ManiaPlanet)
            .WithOne(x => x.Account)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AccountModel>()
            .HasOne(x => x.Trackmania)
            .WithOne(x => x.Account)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AccountModel>()
            .HasOne(x => x.Discord)
            .WithOne(x => x.Account)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AccountModel>()
            .HasOne(x => x.GitHub)
            .WithOne(x => x.Account)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AccountModel>()
            .HasOne(x => x.Twitter)
            .WithOne(x => x.Account)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
