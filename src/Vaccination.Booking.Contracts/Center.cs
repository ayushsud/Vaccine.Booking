using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Contracts
{
    public class Center
    {
        public int center_id { get; set; }
        public string pincode { get; set; }
        public List<Session> sessions{ get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string block_name { get; set; }
    }
}
