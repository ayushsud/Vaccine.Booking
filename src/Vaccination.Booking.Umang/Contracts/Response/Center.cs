using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class Center
    {
        public string pincode { get; set; }
        public List<Session> sessions{ get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string block_name { get; set; }
        public int center_id { get; set; }
    }
}
