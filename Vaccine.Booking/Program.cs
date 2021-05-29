using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;
using Vaccination.Booking.Umang;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccine.Booking
{
    static class Program
    {
        private static ServiceProvider _serviceProvider;
        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 5;
            RegisterServices();
            var scope = _serviceProvider.CreateScope();
            var scheduleAppointmentService = scope.ServiceProvider.GetRequiredService<IScheduleAppointmentService>();
            try
            {
                scheduleAppointmentService.ScheduleAppointmentAsync().GetAwaiter().GetResult();
            }
            catch(TaskCanceledException)
            {
                Console.WriteLine("\nCheck registered mobile for center details.");
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IScheduleAppointmentService, ScheduleAppointmentService>();
            services.AddSingleton<IProfileService, FileBasedProfileService>();
            services.AddSingleton<IPinCodeProvider, FileBasedPinCodeProvider>();
            services.AddSingleton<IUmangTokenProvider, UmangTokenProvider>();
            _serviceProvider = services.BuildServiceProvider(true);
        }
    }
}
