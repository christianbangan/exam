using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Reservation
    {
        public int TransactionId { get; set; }
        public string CustomerName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReservedSeat { get; set; }
        public int SeatId { get; set; }
        public string SeatValue { get; set; }
        public string Status { get; set; }
        public int MovieId { get; set; }
    }
}