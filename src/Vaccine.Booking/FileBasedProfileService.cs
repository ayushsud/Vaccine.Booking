using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using Vaccination.Booking.Contracts;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccine.Booking
{
    public class FileBasedProfileService : IProfileService
    {
        private readonly string _filePath;
        public FileBasedProfileService(IOptions<FilePathConfigurations> filePaths)
        {
            _filePath = filePaths.Value.Profile;
        }
        public Profile GetProfile()
        {
            try
            {
                var data = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<Profile>(data);
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to read profile.json");
                Console.ResetColor();
                throw;
            }
        }
    }
}
