using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccination.Booking.Umang
{
    public class FileBasedProfileService : IProfileService
    {
        public Profile GetProfile()
        {
            try
            {
                var data = File.ReadAllText(Constants.FilePaths.Profile);
                return JsonConvert.DeserializeObject<Profile>(data);
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to read pin_codes.json");
                Console.ResetColor();
                throw;
            }
        }
    }
}
