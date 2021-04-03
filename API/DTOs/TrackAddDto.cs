using System.ComponentModel.DataAnnotations;

namespace API.DTOs {
    public class TrackAddDto {

        [Required]
        public string URL { get; set; }
        [Required]
        public ulong DiscordId { get; set; }
        public string Username { get; set; }
    }
}