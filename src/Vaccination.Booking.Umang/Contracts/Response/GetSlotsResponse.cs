using Vaccination.Booking.Contracts;

namespace Vaccination.Booking.Umang.Contracts
{
    public class GetSlotsResponse
    {
        public string rs { get; set; }
        public string rc { get; set; }
        public string rd { get; set; }
        public SlotData pd { get; set; }
    }
}
