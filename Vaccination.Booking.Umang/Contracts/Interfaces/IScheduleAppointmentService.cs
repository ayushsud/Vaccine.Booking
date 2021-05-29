using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vaccination.Booking.Umang.Contracts
{
    public interface IScheduleAppointmentService
    {
        Task ScheduleAppointmentAsync();
    }
}
