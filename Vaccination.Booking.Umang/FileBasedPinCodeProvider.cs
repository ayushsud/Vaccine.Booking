using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccination.Booking.Umang
{
    public class FileBasedPinCodeProvider : IPinCodeProvider
    {
        public List<string> GetPinCodes()
        {
            try
            {
                var data = File.ReadAllText(Constants.FilePaths.PinCodes);
                return JsonConvert.DeserializeObject<List<string>>(data);
            }
            catch(Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to read pin_codes.json");
                Console.ResetColor();
                throw;
            }
        }
    }
}
