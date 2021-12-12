using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter title.")]
        [StringLength(255)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter description.")]
        [StringLength(255)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please upload an item's image (NOTE: only KNOWN image formats will be accepted)")]
        [StringLength(500)]
        public string image_path { get; set; }

        public List<Schedule> Schedules { get; set; }

        public List<int> AvailableSeats { get; set; }

        public List<Seat> Seats { get; set; }

        public string Status { get; set; }

        public List<Reservation> Reservations { get; set; }

        public Dictionary<string, int> AvailableSeatsDictionary { get; set; }

    }
}