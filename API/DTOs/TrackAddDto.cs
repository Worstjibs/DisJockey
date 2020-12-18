using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs {
    public class TrackAddDto {

        [Required]
        public string URL { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public UserDto user { get; set; }
    }
}