using System;
using System.Collections.Generic;
using System.Text;

namespace Vaccination.Booking.Umang.Contracts
{
    public class Profile
    {
        public int DistrictId { get; set; }
        public string Date { get; set; }
        public string Mobile { get; set; }
        public string Mpin { get; set; }
        public List<string> Beneficiaries { get; set; }
    }
}
