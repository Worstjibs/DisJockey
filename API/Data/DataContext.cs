using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data {
    public class DataContext : DbContext {
        public DataContext(DbContextOptions options): base(options) {

        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<AppUserTrack> UserTracks { get; set; }
        public DbSet<TrackLike> TrackLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            // AppUserTrack configuration:
            // Add Primary key
            builder.Entity<AppUserTrack>()
                .HasKey(k => new { k.AppUserId, k.TrackId });
            // Add Foreign keys for AppUser
            builder.Entity<AppUserTrack>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.Tracks)
                .HasForeignKey(ut => ut.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
            // Add Foreign keys for Track
            builder.Entity<AppUserTrack>()
                .HasOne(ut => ut.Track)
                .WithMany(u => u.AppUsers)
                .HasForeignKey(ut => ut.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            // TrackLike configuration
            // Add Pimrary key
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
        }
    }
}