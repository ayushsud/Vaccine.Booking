using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Cowin.Contracts
{
    public class ValidateOtpResponse
    {
        public string token { get; set; }
        public char isNewAccount { get; set; }
    }
}
