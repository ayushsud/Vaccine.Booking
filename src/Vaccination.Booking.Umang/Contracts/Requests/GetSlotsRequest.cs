using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class GetSlotsRequest : CowinBaseRequest
    {
        public int district_id { get; set; }
        public string date { get; set; }
        public string vaccine { get; set; }
        public string token { get; set; }

    }
}
