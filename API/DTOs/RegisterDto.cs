using System.ComponentModel.DataAnnotations;

namespace API.DTOs {
    public class RegisterDto {
        [Required]
        public string Username { get; set; }
        [Required]
        public string DiscordId { get; set; }
    }
}