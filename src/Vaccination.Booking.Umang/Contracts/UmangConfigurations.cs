using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class UmangConfigurations
    {
        public string BaseUrl { get; set; }
        public BearerTokens BearerTokens { get; set; }
        public Headers Headers { get; set; }
    }

    public class BearerTokens
    {
        public string Cowin { get; set; }
        public string Umang { get; set; }
    }

    public class Headers
    {
        public string Host { get; set; }
    }
}
