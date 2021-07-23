using System.Collections.Generic;
using DisJockey.Core;

namespace DisJockey.Data
{
    public class SeedData
    {
        public ICollection<AppUser> Users { get; set; }
        public ICollection<Track> Tracks { get; set; }
    }
}