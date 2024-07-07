using DisJockey.Core;

namespace DisJockey.Infrastructure;

public class SeedData
{
    public ICollection<AppUser> Users { get; set; }
    public ICollection<Track> Tracks { get; set; }
}