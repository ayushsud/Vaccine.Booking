using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Text;
using Vaccination.Booking.Contracts;
using Vaccination.Booking.Cowin;
using Vaccination.Booking.Umang;

namespace Vaccine.Booking
{
    public class CowinAppointmentServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public CowinAppointmentServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IScheduleAppointmentService CreateServiceInstance(int type)
        {
            switch (type)
            {
                case 1:
                    return _serviceProvider.GetService<ScheduleCowinAppointmentService>();
                case 2:
                    return _serviceProvider.GetService<ScheduleUmangAppointmentService>();
                default:
                    return _serviceProvider.GetService<ScheduleCowinAppointmentService>();
            }
        }
    }
}
