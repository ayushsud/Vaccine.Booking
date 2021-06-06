using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Contracts
{
    public interface IPinCodeProvider
    {
        List<string> GetPinCodes();
    }
}
