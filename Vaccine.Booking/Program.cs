using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http.Headers;
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
            catch (TaskCanceledException)
            {
                Console.WriteLine("\nCheck registered mobile for center details.");
            }
            Console.WriteLine("Exiting!");
            Console.ReadKey();
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddHttpClient<ICowinHttpClient, CowinHttpClient>(client =>
            {
                client.BaseAddress = new Uri(Constants.URLs.BaseUrl);
                 #region add headers
                 client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.BearerTokens.Cowin);
                client.DefaultRequestHeaders.Connection.Clear();
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                client.DefaultRequestHeaders.Add("deptid", "355");
                client.DefaultRequestHeaders.Add("formtrkr", "0");
                client.DefaultRequestHeaders.Add("srvid", "1604");
                client.DefaultRequestHeaders.Add("subsid", "0");
                client.DefaultRequestHeaders.Add("subsid2", "0");
                client.DefaultRequestHeaders.Add("tenantId", string.Empty);
                 #endregion
             });
            services.AddHttpClient<IUmangHttpClient, UmangHttpClient>(client =>
            {
                client.BaseAddress = new Uri(Constants.URLs.BaseUrl);
                #region add headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.BearerTokens.Umang);
                client.DefaultRequestHeaders.Connection.Clear();
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                #endregion
            });
            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddSingleton<IScheduleAppointmentService, ScheduleAppointmentService>();
            services.AddSingleton<IProfileService, FileBasedProfileService>();
            services.AddSingleton<IPinCodeProvider, FileBasedPinCodeProvider>();
            services.AddSingleton<IUmangTokenProvider, UmangTokenProvider>();
            _serviceProvider = services.BuildServiceProvider(true);
        }
    }
}
