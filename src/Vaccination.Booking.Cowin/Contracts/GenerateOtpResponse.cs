using Newtonsoft.Json;

namespace Vaccination.Booking.Cowin.Contracts
{
    public class GenerateOtpResponse
    {
        [JsonProperty("txnId")]
        public string TransactionId{ get; set; }
    }
}
