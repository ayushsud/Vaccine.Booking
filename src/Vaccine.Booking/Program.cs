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
            var authenticationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
            Console.WriteLine("Enter App Password");
            string password = Console.ReadLine();
            if (!authenticationService.Authorize(password).GetAwaiter().GetResult())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unauthorized access!");
                Console.ResetColor();
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Authorization Successful!");
                Console.ResetColor();
            }
            Console.WriteLine("\nEnter 1 for Cowin\nEnter 2 for Umang");
            int serviceType = Console.ReadKey().KeyChar - '0';
            var scheduleAppointmentService = new CowinAppointmentServiceFactory(scope.ServiceProvider).CreateServiceInstance(serviceType);
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
            services.AddSingleton<ScheduleCowinAppointmentService>();
            services.AddSingleton<ScheduleUmangAppointmentService>();
            UmangConfigurations umangConfigs = new UmangConfigurations();
            configuration.GetSection("Umang").Bind(umangConfigs);
            services.AddHttpClient<Vaccination.Booking.Umang.Contracts.ICowinHttpClient, Vaccination.Booking.Umang.CowinHttpClient>("umangCowin", client =>
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
                client.DefaultRequestHeaders.Add("Host", umangConfigs.Headers.Host);
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
                client.DefaultRequestHeaders.Add("Host", umangConfigs.Headers.Host);
                #endregion
            });
            CowinConfigurations cowinConfigs = new CowinConfigurations();
            configuration.GetSection("Cowin").Bind(cowinConfigs);
            services.AddHttpClient<Vaccination.Booking.Cowin.Contracts.ICowinHttpClient, Vaccination.Booking.Cowin.CowinHttpClient>("cowin", client =>
             {
                 client.BaseAddress = new Uri(cowinConfigs.BaseUrl);
                 client.DefaultRequestHeaders.Add("Host", cowinConfigs.Headers.Host);
                 client.DefaultRequestHeaders.Add("origin", cowinConfigs.Headers.Origin);
             });
            string authenticationUrl = configuration.GetValue<string>("AuthenticationUrl");
            services.AddHttpClient<IAuthorizationService, AuthorizationService>(client =>
                    client.BaseAddress = new Uri(authenticationUrl));
            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddSingleton<IProfileService, FileBasedProfileService>();
            services.AddSingleton<IPinCodeProvider, FileBasedPinCodeProvider>();
            services.AddSingleton<IUmangTokenProvider, UmangTokenProvider>();
            _serviceProvider = services.BuildServiceProvider(true);
        }
    }
}
