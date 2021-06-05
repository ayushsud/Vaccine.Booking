using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Vaccination.Booking.Contracts;
using Vaccination.Booking.Umang;
using Vaccination.Booking.Umang.Contracts;
using Microsoft.Extensions.Configuration;
using Vaccination.Booking.Cowin.Contracts;
using Vaccination.Booking.Cowin;

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
            var profile = scope.ServiceProvider.GetRequiredService<IProfileService>().GetProfile();
            var pinCodes = scope.ServiceProvider.GetRequiredService<IPinCodeProvider>().GetPinCodes();
            try
            {
                scheduleAppointmentService.ScheduleAppointmentAsync(profile, pinCodes).GetAwaiter().GetResult();
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
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            services.AddOptions();
            services.Configure<FilePathConfigurations>(configuration.GetSection("FilePaths"));
            services.AddSingleton<IConfiguration>(configuration);
            UmangConfigurations umangConfigs = new UmangConfigurations();
            configuration.GetSection("Umang").Bind(umangConfigs);
            services.AddHttpClient<ICowinHttpClient, CowinHttpClient>(client =>
            {
                client.BaseAddress = new Uri(umangConfigs.BaseUrl);
                #region add headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Vaccination.Booking.Umang.Constants.ApplicationJson));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", umangConfigs.BearerTokens.Cowin);
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
                client.BaseAddress = new Uri(umangConfigs.BaseUrl);
                #region add headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Vaccination.Booking.Umang.Constants.ApplicationJson));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", umangConfigs.BearerTokens.Umang);
                client.DefaultRequestHeaders.Connection.Clear();
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                #endregion
            });
            CowinConfigurations cowinConfigs = new CowinConfigurations();
            configuration.GetSection("Cowin").Bind(cowinConfigs);
            services.AddHttpClient<IScheduleAppointmentService, ScheduleCowinAppointmentService>(client=>
            {
                client.BaseAddress = new Uri(cowinConfigs.BaseUrl);
                client.DefaultRequestHeaders.Add("Host", cowinConfigs.Headers.Host);
                client.DefaultRequestHeaders.Add("origin", cowinConfigs.Headers.Origin);
            });
            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddSingleton<IProfileService, FileBasedProfileService>();
            services.AddSingleton<IPinCodeProvider, FileBasedPinCodeProvider>();
            services.AddSingleton<IUmangTokenProvider, UmangTokenProvider>();
            _serviceProvider = services.BuildServiceProvider(true);
        }
    }
}
