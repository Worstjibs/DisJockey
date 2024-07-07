using DisJockey.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DisJockey.Infrastructure;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<TrackPlay> TrackPlays { get; set; }
    public DbSet<TrackLike> TrackLikes { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistTrack> PlaylistTracks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Track configuration
        builder.Entity<Track>()
            .HasMany(t => t.Playlists)
            .WithOne(tp => tp.Track)
            .HasForeignKey(tp => tp.TrackId);

        builder.Entity<Track>()
            .HasQueryFilter(x => !x.Blacklisted);

        // AppUserTrack configuration:
        // Add Foreign keys for AppUser
        builder.Entity<TrackPlay>()
            .HasOne(ut => ut.User)
            .WithMany(u => u.Tracks)
            .HasForeignKey(ut => ut.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Add Foreign keys for Track
        builder.Entity<TrackPlay>()
            .HasOne(ut => ut.Track)
            .WithMany(u => u.TrackPlays)
            .HasForeignKey(ut => ut.TrackId)
            .OnDelete(DeleteBehavior.Cascade);

        // TrackLike configuration
        // Add Primary key
        builder.Entity<TrackLike>()
            .HasKey(k => new { k.UserId, k.TrackId });
        // Add Foreign keys for AppUser
        builder.Entity<TrackLike>()
            .HasOne(tl => tl.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(tl => tl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Add Foreign keys for Track
        builder.Entity<TrackLike>()
            .HasOne(tl => tl.Track)
            .WithMany(t => t.Likes)
            .HasForeignKey(tl => tl.TrackId)
            .OnDelete(DeleteBehavior.Cascade);

        // Playlist configuration
        // Add Primary key
        builder.Entity<PlaylistTrack>()
            .HasKey(k => new { k.TrackId, k.PlaylistId });
        // Add Foreign keys to Track
        builder.Entity<PlaylistTrack>()
            .HasOne(pt => pt.Track)
            .WithMany(t => t.Playlists)
            .HasForeignKey(pt => pt.TrackId)
            .OnDelete(DeleteBehavior.Cascade);
        // Add Foreign keys to Playlist
        builder.Entity<PlaylistTrack>()
            .HasOne(pt => pt.Playlist)
            .WithMany(p => p.Tracks)
            .HasForeignKey(pt => pt.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        // PullUp configuration
        builder.Entity<PullUp>()
            .HasOne(pu => pu.Track)
            .WithMany(t => t.PullUps)
            .HasForeignKey(pu => pu.TrackId);

        builder.Entity<PullUp>()
            .HasOne(pu => pu.User)
            .WithMany(u => u.PullUps)
            .HasForeignKey(pu => pu.UserId);

        builder.ApplyUtcDateTimeConverter();
    }
}

public static class UtcDateAnnotation
{
    private const string IsUtcAnnotation = "IsUtc";
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
      new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
      new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

    public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, bool isUtc = true) =>
      builder.HasAnnotation(IsUtcAnnotation, isUtc);

    public static bool IsUtc(this IMutableProperty property) =>
      ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

    /// <summary>
    /// Make sure this is called after configuring all your entities.
    /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (!property.IsUtc())
                {
                    continue;
                }

                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(UtcConverter);
                }

                if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(UtcNullableConverter);
                }
            }
        }
    }
}