using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class ConfirmOtpRequest:CowinBaseRequest
    {
        public string otp { get; set; }
        public string txnId { get; set; }
    }
}
