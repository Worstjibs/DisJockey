using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs {
    public class TrackDto {
        public string URL { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}