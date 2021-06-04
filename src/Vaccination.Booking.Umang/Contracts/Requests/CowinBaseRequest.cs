using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class CowinBaseRequest
    {
        public string tkn { get; set; }
        public string lang { get; set; } = "en";
        public string language { get; set; } = "en";
        public string usrid { get; set; }
        public string mode { get; set; } = "web";
        public string pltfrm { get; set; } = "windows";
        public string did { get; set; } = null;
        public string deptid { get; set; } = "355";
        public string formtrkr { get; set; } = "0";
        public string srvid { get; set; } = "1604";
        public string subsid { get; set; } = "0";
        public string subsid2 { get; set; } = "0";
    }
}
