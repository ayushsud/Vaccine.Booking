using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang
{
    public static class Constants
    {
        public static class URLs
        {
            public static readonly string BaseUrl = string.Empty;
            public static readonly string Core = "coreapi/opn/ws2/";
            public static readonly string Cowin = "depttapi/COWINApi/ws1/1.0/v2/";
            public static class Verbs
            {
                public static readonly string UmangLogin = "openlgv1";
                public static readonly string GenerateOtp = "generateOTP";
                public static readonly string ConfirmOtp = "confirmOTP";
                public static readonly string GetBeneficiaries = "beneficiaries";
                public static readonly string GetSlots = "calendarByDistrict";
                public static readonly string ScheduleAppointment = "scheduleappointment";
            }
        }

        public static class BearerTokens
        {
            public static readonly string Cowin = string.Empty;
            public static readonly string Umang = string.Empty;
        }

        public static class FilePaths
        {
            public static readonly string Profile = "profile.json";
            public static readonly string PinCodes = "pin_codes.json";
        }

        public static readonly string ApplicationJson = "application/json";
    }
}
