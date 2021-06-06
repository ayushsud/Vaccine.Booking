using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vaccination.Booking.Contracts;
using Vaccination.Booking.Cowin.Contracts;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Linq;
using System.Threading;

namespace Vaccination.Booking.Cowin
{
    public class ScheduleCowinAppointmentService : IScheduleAppointmentService
    {
        private readonly HttpClient _httpClient;
        private bool _isSlotBooked;
        private string _token;
        private List<string> _beneficiaries;
        private List<string> _centerPriorityList;
        private readonly CancellationTokenSource cancellationTokenSource;

        public ScheduleCowinAppointmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _isSlotBooked = false;
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task ScheduleAppointmentAsync(Profile profile, List<string> pinCodes)
        {
            _beneficiaries = profile.Beneficiaries;
            _centerPriorityList = pinCodes;
            var token = await GetTokenAsync(profile.Mobile);
            if (!string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("\nTrying to book slots...");
                _token = token;
                while (!_isSlotBooked)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                    var queryparams = new Dictionary<string, string>{
                        {"district_id",profile.DistrictId.ToString() },
                        {"date",profile.Date } };
                    var res = await _httpClient.GetAsync(QueryHelpers.AddQueryString(Constants.URLs.GetSlotsPublic, queryparams), cancellationTokenSource.Token);
                    if (res.IsSuccessStatusCode)
                    {
                        var slots = JsonConvert.DeserializeObject<SlotData>(await res.Content.ReadAsStringAsync());
                        if (slots.centers.Count > 0)
                        {
                            await FindEligibleCentersAsync(slots.centers);
                        }
                    }
                    else
                    {
                        Console.WriteLine(await res.Content.ReadAsStringAsync());
                        _isSlotBooked = true;
                    }
                }
            }
        }

        private async Task<string> GetTokenAsync(string mobile)
        {
            var generateOtpRequest = new StringContent(JsonConvert.SerializeObject(new GenerateOtpRequest
            {
                mobile = mobile
            }), Encoding.UTF8, Constants.ApplicationJson);
            var res = await _httpClient.PostAsync(Constants.URLs.GenerateOtp, generateOtpRequest);
            if (res.IsSuccessStatusCode)
            {
                var generateOtpResponse = JsonConvert.DeserializeObject<GenerateOtpResponse>(await res.Content.ReadAsStringAsync());
                Console.WriteLine("Please Enter Otp");
                string otp = Console.ReadLine();
                string hashedOtp = Utilities.ComputeSha256Hash(otp);
                var validateOtpRequest = new StringContent(JsonConvert.SerializeObject(new ValidateOtpRequest
                {
                    otp = hashedOtp,
                    TransactionId = generateOtpResponse.TransactionId
                }), Encoding.UTF8, Constants.ApplicationJson);
                var httpRes = await _httpClient.PostAsync(Constants.URLs.ValidateOtp, validateOtpRequest);
                if (httpRes.IsSuccessStatusCode)
                {
                    var validateOtpRes = JsonConvert.DeserializeObject<ValidateOtpResponse>(await httpRes.Content.ReadAsStringAsync());
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("OTP Verfied");
                    Console.ResetColor();
                    return validateOtpRes.token;
                }
            }
            return string.Empty;
        }

        private async Task FindEligibleCentersAsync(List<Center> centers)
        {
            List<Task> tasks = new List<Task>();
            _centerPriorityList.ForEach(pincode =>
            {
                var matches = centers.FindAll(center => center.pincode == pincode &&
                center.sessions.Any(session => session.available_capacity >= _beneficiaries.Count &&
                session.available_capacity_dose1 >= _beneficiaries.Count &&
                session.min_age_limit == 18));
                if (matches?.Count > 0)
                {
                    matches.ForEach(match =>
                    match.sessions.FindAll(session => session.min_age_limit == 18 &&
                    session.available_capacity >= _beneficiaries.Count &&
                    session.available_capacity_dose1 >= _beneficiaries.Count)
                    .ForEach(x =>
                    {
                        tasks.Add(BookSlotAsync(x.session_id));
                    }));
                }
            });
            if (tasks.Count == 0)
                await Task.Delay(500);
            await Task.WhenAll(tasks);
        }

        private async Task BookSlotAsync(string sessionId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var req = new StringContent(JsonConvert.SerializeObject(new ScheduleAppointmentRequest
            {
                beneficiaries = _beneficiaries,
                session_id = sessionId
            }), Encoding.UTF8, Constants.ApplicationJson);
            var res = await _httpClient.PostAsync(Constants.URLs.ScheduleAppointment, req, cancellationTokenSource.Token);
            if(res.IsSuccessStatusCode)
            {
                _isSlotBooked = true;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nSuccess! Slot booked succeesfuly.");
                Console.ResetColor();
                cancellationTokenSource.Cancel();
            }
        }
    }
}
