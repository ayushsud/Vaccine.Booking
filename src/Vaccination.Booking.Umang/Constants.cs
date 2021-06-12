using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang
{
    public static class Constants
    {
        public static class URLs
        {
            public static readonly string Core = "coreapi/opn/ws2/";
            public static readonly string Cowin = "depttapi/COWINApi/ws1/1.0/v2/";
            public static class Verbs
            {
                public static readonly string UmangLogin = "openlgv1";
                public static readonly string GenerateOtp = "generateOTP";
                public static readonly string ConfirmOtp = "confirmOTP";
                public static readonly string GetBeneficiaries = "beneficiaries";
                public static readonly string GetSlots = "calendarByDistrict";
                public static readonly string GetSlotsPublic = "pub/calendarByDistrict";
                public static readonly string ScheduleAppointment = "scheduleappointment";
            }
        }

        public static readonly string ApplicationJson = "application/json";
    }
}
