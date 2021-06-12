using System.Net.Http;
using System.Threading.Tasks;
using Vaccination.Booking.Contracts;
using Vaccination.Booking.Cowin;

namespace Vaccine.Booking
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly HttpClient _httpClient;

        public AuthorizationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Authorize(string password)
        {
            string passwordHash = Utilities.ComputeSha256Hash(password);
            var httpRes = await _httpClient.GetAsync(string.Empty);
            if (httpRes.IsSuccessStatusCode)
                return string.Equals(await httpRes.Content.ReadAsStringAsync(), passwordHash);
            return false;
        }
    }
}
