using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Cowin.Contracts
{
    public class GenerateOtpRequest
    {
        public string mobile { get; set; }
        public string secret { get; set; } = "U2FsdGVkX198GQCa6+ZpZTmi/T2Wqf5G+HR2r/OSmrK+3P38v5bdXOe+fsW1gcwMqjTSegOaBJFefUd8/HnSeQ==";
    }
}
