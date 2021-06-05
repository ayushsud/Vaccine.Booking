using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Cowin.Contracts
{
    public class ScheduleAppointmentRequest
    {
        public List<string> beneficiaries { get; set; }
        public int dose { get; set; } = 1;
        public string session_id { get; set; }
        public string slot { get; set; } = "09:00AM-10:00AM";
    }
}
