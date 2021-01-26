using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs {
    public class TrackAddDto {

        [Required]
        public string URL { get; set; }
        public long DiscordId { get; set; }
    }
}