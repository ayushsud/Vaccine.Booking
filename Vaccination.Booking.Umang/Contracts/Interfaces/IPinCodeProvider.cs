﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public interface IPinCodeProvider
    {
        List<string> GetPinCodes();
    }
}
