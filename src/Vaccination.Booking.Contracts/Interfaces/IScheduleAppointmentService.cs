using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vaccination.Booking.Contracts
{
    public interface IScheduleAppointmentService
    {
        Task ScheduleAppointmentAsync(Profile profile, List<string> pinCodes);
    }
}
