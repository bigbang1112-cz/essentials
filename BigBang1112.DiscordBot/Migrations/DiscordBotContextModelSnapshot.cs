﻿// <auto-generated />
using System;
using BigBang1112.DiscordBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BigBang1112.DiscordBot.Migrations
{
    [DbContext(typeof(DiscordBotContext))]
    partial class DiscordBotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotCommandModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BotId")
                        .HasColumnType("int");

                    b.Property<string>("CommandName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("LastUsedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Used")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BotId");

                    b.ToTable("DiscordBotCommands");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotCommandVisibilityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<int>("JoinedGuildId")
                        .HasColumnType("int");

                    b.Property<bool>("Visibility")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("JoinedGuildId");

                    b.ToTable("DiscordBotCommandVisibilities");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotGuildModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<ulong>("Snowflake")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("DiscordBotGuilds");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotChannelModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("GuildId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<ulong>("Snowflake")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("DiscordBotChannels");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotJoinedGuildModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BotId")
                        .HasColumnType("int");

                    b.Property<bool>("CommandVisibility")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("GuildId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BotId");

                    b.HasIndex("GuildId");

                    b.ToTable("DiscordBotJoinedGuilds");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<Guid>("Guid")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("DiscordBots");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordUserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BotId")
                        .HasColumnType("int");

                    b.Property<ushort>("Discriminator")
                        .HasColumnType("smallint unsigned");

                    b.Property<DateTime>("FirstInteractionOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Interactions")
                        .HasColumnType("int");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("LastInteractionOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<ulong>("Snowflake")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("BotId");

                    b.ToTable("DiscordUsers");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.FeedbackModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Responded")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("varchar(5000)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("WrittenOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.MemeModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime?>("AddedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Attachment")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<ulong?>("AuthorSnowflake")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("Guid")
                        .HasColumnType("char(36)");

                    b.Property<int>("JoinedGuildId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("JoinedGuildId");

                    b.ToTable("Memes");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.PingMessageModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("varchar(2000)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("WrittenOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("PingMessages");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.ReportChannelMessageModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<ulong>("MessageId")
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<bool>("RemovedByUser")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("RemovedOfficially")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ReportGuid")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("SentOn")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("ReportChannelMessages");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.ReportChannelModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<int>("JoinedGuildId")
                        .HasColumnType("int");

                    b.Property<string>("Scope")
                        .HasColumnType("longtext");

                    b.Property<string>("ThreadOptions")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("JoinedGuildId");

                    b.ToTable("ReportChannels");
                });

            modelBuilder.Entity("BigBang1112.UniReminder.Models.PredmetModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<Guid>("Guid")
                        .HasColumnType("char(36)");

                    b.Property<byte>("Kredity")
                        .HasColumnType("tinyint unsigned");

                    b.Property<bool>("LS")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Nazev")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Pracoviste")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Predmet")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TypZkousky")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("ZS")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Predmety");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotCommandModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotModel", "Bot")
                        .WithMany()
                        .HasForeignKey("BotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bot");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotCommandVisibilityModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotChannelModel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotJoinedGuildModel", "JoinedGuild")
                        .WithMany("CommandVisibilities")
                        .HasForeignKey("JoinedGuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("JoinedGuild");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotChannelModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotGuildModel", "Guild")
                        .WithMany("Channels")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotJoinedGuildModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotModel", "Bot")
                        .WithMany("JoinedGuilds")
                        .HasForeignKey("BotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotGuildModel", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bot");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordUserModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotModel", "Bot")
                        .WithMany()
                        .HasForeignKey("BotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bot");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.FeedbackModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordUserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.MemeModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotJoinedGuildModel", "JoinedGuild")
                        .WithMany()
                        .HasForeignKey("JoinedGuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JoinedGuild");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.PingMessageModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordUserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.ReportChannelMessageModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.ReportChannelModel", "Channel")
                        .WithMany("Messages")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.ReportChannelModel", b =>
                {
                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotChannelModel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BigBang1112.DiscordBot.Models.Db.DiscordBotJoinedGuildModel", "JoinedGuild")
                        .WithMany()
                        .HasForeignKey("JoinedGuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("JoinedGuild");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotGuildModel", b =>
                {
                    b.Navigation("Channels");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotJoinedGuildModel", b =>
                {
                    b.Navigation("CommandVisibilities");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.DiscordBotModel", b =>
                {
                    b.Navigation("JoinedGuilds");
                });

            modelBuilder.Entity("BigBang1112.DiscordBot.Models.Db.ReportChannelModel", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
