using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Cowin
{
    public static class Constants
    {
        public static readonly string ApplicationJson = "application/json";
        public static class URLs
        {
            public static readonly string GenerateOtp = "auth/generateMobileOTP";
            public static readonly string ValidateOtp = "auth/validateMobileOtp";
            public static readonly string GetSlots = "appointment/sessions/calendarByDistrict";
            public static readonly string GetSlotsPublic = "appointment/sessions/public/calendarByDistrict";
            public static readonly string ScheduleAppointment = "appointment/schedule";
        }
    }
}
