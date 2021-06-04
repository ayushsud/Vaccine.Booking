using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class ScheduleAppointmentRequest : CowinBaseRequest
    {
        public string token { get; set; }
        public string slot { get; set; } = "10:00AM-11:00AM";
        public List<string> beneficiaries { get; set; }
        public string session_id { get; set; }
        public int dose { get; set; } = 1;
        public string lat { get; set; } = "0";
        public string lon { get; set; } = "0";
        public string lac { get; set; } = "0";
        public string usag { get; set; } = "0";
    }
}
