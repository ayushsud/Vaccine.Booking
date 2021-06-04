using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class GetBeneficiariesRequest : CowinBaseRequest
    {
        public string token { get; set; }
    }
}
