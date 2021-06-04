using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Vaccination.Booking.Contracts;

namespace Vaccine.Booking
{
    public class FileBasedPinCodeProvider : IPinCodeProvider
    {
        private readonly string _filePath;
        public FileBasedPinCodeProvider(IOptions<FilePathConfigurations> filePaths)
        {
            _filePath = filePaths.Value.PinCodes;
        }
        public List<string> GetPinCodes()
        {
            try
            {
                var data = File.ReadAllText(_filePath);
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
