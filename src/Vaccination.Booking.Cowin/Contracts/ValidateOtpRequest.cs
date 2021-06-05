using Newtonsoft.Json;

namespace Vaccination.Booking.Cowin.Contracts
{
    class ValidateOtpRequest
    {
        public string otp { get; set; }
        [JsonProperty("txnId")]
        public string TransactionId { get; set; }
    }
}
