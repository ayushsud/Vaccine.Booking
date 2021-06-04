using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class GenerateOtpRequest : CowinBaseRequest
    {
        public string mobile { get; set; }
    }
}
