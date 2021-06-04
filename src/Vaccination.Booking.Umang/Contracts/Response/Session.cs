using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class Session
    {
        public string date { get; set; }
        public string vaccine { get; set; }
        public string session_id { get; set; }
        public int min_age_limit { get; set; }
        public int available_capacity { get; set; }
        public int available_capacity_dose1 { get; set; }
        public int available_capacity_dose2 { get; set; }
    }
}
