﻿// <auto-generated />
using System;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20210709192454_Added relation from Users to Playlists")]
    partial class AddedrelationfromUserstoPlaylists
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4");

            modelBuilder.Entity("API.Entities.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("API.Entities.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AppUserId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("API.Entities.PlaylistTrack", b =>
                {
                    b.Property<int>("TrackId")
                        .HasColumnType("int");

                    b.Property<int>("PlaylistId")
                        .HasColumnType("int");

                    b.Property<int?>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("TrackId", "PlaylistId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("PlaylistId");

                    b.ToTable("PlaylistTracks");
                });

            modelBuilder.Entity("API.Entities.PullUp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<double>("TimePulled")
                        .HasColumnType("float");

                    b.Property<int>("TrackId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TrackId");

                    b.HasIndex("UserId");

                    b.ToTable("PullUps");
                });

            modelBuilder.Entity("API.Entities.Track", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ChannelTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LargeThumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MediumThumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmallThumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("YoutubeId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("API.Entities.TrackLike", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("TrackId")
                        .HasColumnType("int");

                    b.Property<bool>("Liked")
                        .HasColumnType("bit");

                    b.HasKey("UserId", "TrackId");

                    b.HasIndex("TrackId");

                    b.ToTable("TrackLikes");
                });

            modelBuilder.Entity("API.Entities.TrackPlay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastPlayed")
                        .HasColumnType("datetime2");

                    b.Property<int>("TrackId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("TrackId");

                    b.ToTable("TrackPlays");
                });

            modelBuilder.Entity("API.Entities.TrackPlayHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TrackPlayId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TrackPlayId");

                    b.ToTable("TrackPlayHistory");
                });

            modelBuilder.Entity("API.Entities.Playlist", b =>
                {
                    b.HasOne("API.Entities.AppUser", null)
                        .WithMany("Playlists")
                        .HasForeignKey("AppUserId");
                });

            modelBuilder.Entity("API.Entities.PlaylistTrack", b =>
                {
                    b.HasOne("API.Entities.AppUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("API.Entities.Playlist", "Playlist")
                        .WithMany("Tracks")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entities.Track", "Track")
                        .WithMany("Playlists")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("Playlist");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("API.Entities.PullUp", b =>
                {
                    b.HasOne("API.Entities.Track", "Track")
                        .WithMany("PullUps")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entities.AppUser", "User")
                        .WithMany("PullUps")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Track");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Entities.TrackLike", b =>
                {
                    b.HasOne("API.Entities.Track", "Track")
                        .WithMany("Likes")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entities.AppUser", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Track");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Entities.TrackPlay", b =>
                {
                    b.HasOne("API.Entities.AppUser", "User")
                        .WithMany("Tracks")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entities.Track", "Track")
                        .WithMany("TrackPlays")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Track");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Entities.TrackPlayHistory", b =>
                {
                    b.HasOne("API.Entities.TrackPlay", "TrackPlay")
                        .WithMany("TrackPlayHistory")
                        .HasForeignKey("TrackPlayId");

                    b.Navigation("TrackPlay");
                });

            modelBuilder.Entity("API.Entities.AppUser", b =>
                {
                    b.Navigation("Likes");

                    b.Navigation("Playlists");

                    b.Navigation("PullUps");

                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("API.Entities.Playlist", b =>
                {
                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("API.Entities.Track", b =>
                {
                    b.Navigation("Likes");

                    b.Navigation("Playlists");

                    b.Navigation("PullUps");

                    b.Navigation("TrackPlays");
                });

            modelBuilder.Entity("API.Entities.TrackPlay", b =>
                {
                    b.Navigation("TrackPlayHistory");
                });
#pragma warning restore 612, 618
        }
    }
}
