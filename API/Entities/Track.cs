using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities {
    [Table("Tracks")]
    public class Track {
        public int Id { get; set; }
        public string URL { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}