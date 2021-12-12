using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public string ScheduleTime { get; set; }
        public int MovieId { get; set; }
    }
}