using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Cowin.Contracts
{
    public class CowinConfigurations
    {
        public string BaseUrl { get; set; }
        public Headers Headers { get; set; }
    }

    public class Headers
    {
        public string Host { get; set; }
        public string Origin { get; set; }
    }
}
