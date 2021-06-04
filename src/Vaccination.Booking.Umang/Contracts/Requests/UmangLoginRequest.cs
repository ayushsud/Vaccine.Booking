using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class UmangLoginRequest
    {
        public string lang { get; set; } = "en";
        public string type { get; set; } = "mobm";
        public string mod { get; set; } = "web";
        public string acc { get; set; }
        public string ccode { get; set; }
        public string clid { get; set; }
        public string did { get; set; }
        public string hmd { get; set; }
        public string hmk { get; set; }
        public string imei { get; set; }
        public string imsi { get; set; }
        public string lac { get; set; }
        public string lat { get; set; }
        public string lid { get; set; }
        public string lon { get; set; }
        public string mcc { get; set; }
        public string mnc { get; set; }
        public string mno { get; set; }
        public string os { get; set; }
        public string peml { get; set; }
        public string pkg { get; set; }
        public string rot { get; set; }
        public string ver { get; set; }
        public string tkn { get; set; }
        public string node { get; set; }
    }
}
