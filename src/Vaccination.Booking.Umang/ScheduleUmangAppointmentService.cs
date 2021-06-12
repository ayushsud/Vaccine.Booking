using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vaccination.Booking.Contracts;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccination.Booking.Umang
{
    public class ScheduleUmangAppointmentService : IScheduleAppointmentService
    {
        private bool _isSlotBooked;
        private string _umangtoken;
        private string _userId;
        private List<string> _centerPriorityList;
        private List<string> _beneficiaries;
        private readonly CancellationTokenSource tokenSource;
        private readonly ICowinHttpClient _cowinHttpClient;
        private readonly IUmangTokenProvider _umangTokenProvider;

        public ScheduleUmangAppointmentService(ICowinHttpClient cowinHttpClient, IUmangTokenProvider umangTokenProvider)
        {
            _cowinHttpClient = cowinHttpClient;
            _umangTokenProvider = umangTokenProvider;
            _isSlotBooked = false;
            tokenSource = new CancellationTokenSource();
        }

        public async Task ScheduleAppointmentAsync(Profile profile, List<string> pinCodes)
        {
            _centerPriorityList = pinCodes;
            _beneficiaries = profile.Beneficiaries;
            if (string.IsNullOrWhiteSpace(profile.Mobile) ||
                string.IsNullOrWhiteSpace(profile.Mpin) ||
                string.IsNullOrWhiteSpace(profile.Date) ||
                profile.DistrictId < 1 ||
                _beneficiaries.Count == 0 ||
                _centerPriorityList.Count == 0)
                return;
            var umangLoginResponse = await _umangTokenProvider.GetUmangToken(profile.Mobile, profile.Mpin);
            if (!string.IsNullOrWhiteSpace(umangLoginResponse.Token) && !string.IsNullOrWhiteSpace(umangLoginResponse.UserId))
            {
                _umangtoken = umangLoginResponse.Token;
                _userId = umangLoginResponse.UserId;
                var token = await GetToken(profile.Mobile);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("\nTrying to book slots...");
                    while (!_isSlotBooked)
                    {
                        var slotsReq = new StringContent(JsonConvert.SerializeObject(new GetSlotsRequest
                        {
                            tkn = _umangtoken,
                            usrid = _userId,
                            district_id = profile.DistrictId,
                            date = profile.Date
                        }), Encoding.UTF8, Constants.ApplicationJson);
                        var slotsRes = await _cowinHttpClient.PostWithRetryAsync(Constants.URLs.Cowin + Constants.URLs.Verbs.GetSlotsPublic, slotsReq, tokenSource.Token);
                        if (slotsRes.IsSuccessStatusCode)
                        {
                            var res = JsonConvert.DeserializeObject<BaseResponse>(await slotsRes.Content.ReadAsStringAsync());
                            if (IsReponseValid(res))
                            {
                                var slotsData = JsonConvert.DeserializeObject<GetSlotsResponse>(await slotsRes.Content.ReadAsStringAsync());
                                if (slotsData.pd.centers?.Count > 0)
                                {
                                    await FindEligibleCentersAsync(slotsData.pd.centers, token);
                                }
                                else
                                    await Task.Delay(500);
                            }
                            else
                            {
                                _isSlotBooked = true;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\nUnauthenticated access! Please rerun the program.");
                                Console.ResetColor();
                            }
                        }
                    }
                }
            }
        }

        private async Task<string> GetToken(string mobile)
        {
            var generateOtpRequest = new StringContent(JsonConvert.SerializeObject(new GenerateOtpRequest
            {
                tkn = _umangtoken,
                usrid = _userId,
                mobile = mobile
            }), Encoding.UTF8, Constants.ApplicationJson);
            var generateOtpResponse = await _cowinHttpClient.PostWithRetryAsync(Constants.URLs.Cowin + Constants.URLs.Verbs.GenerateOtp, generateOtpRequest);
            if (generateOtpResponse.IsSuccessStatusCode)
            {
                var generateOtpData = JsonConvert.DeserializeObject<BaseResponse>(await generateOtpResponse.Content.ReadAsStringAsync());
                if (IsReponseValid(generateOtpData))
                {
                    string txnId = generateOtpData.pd.txnId;
                    Console.WriteLine("\nPlease Enter Otp");
                    var otp = Console.ReadLine();
                    var confirmOtpRequest = new StringContent(JsonConvert.SerializeObject(new ConfirmOtpRequest
                    {
                        txnId = txnId,
                        otp = otp,
                        tkn = _umangtoken,
                        usrid = _userId
                    }), Encoding.UTF8, Constants.ApplicationJson);
                    var confirmOtpResponse = await _cowinHttpClient.PostWithRetryAsync(Constants.URLs.Cowin + Constants.URLs.Verbs.ConfirmOtp, confirmOtpRequest);
                    if (confirmOtpResponse.IsSuccessStatusCode)
                    {
                        var confirmOtpData = JsonConvert.DeserializeObject<BaseResponse>(await confirmOtpResponse.Content.ReadAsStringAsync());
                        if (IsReponseValid(confirmOtpData))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("OTP Verfied");
                            Console.ResetColor();
                            return confirmOtpData.pd.token;
                        }
                    }
                }
            }
            return string.Empty;
        }

        private async Task FindEligibleCentersAsync(List<Center> centers, string token)
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
                        tasks.Add(BookSlotAsync(x.session_id, token));
                    }));
                }
            });
            if (tasks.Count == 0)
                await Task.Delay(500);
            await Task.WhenAll(tasks);
        }

        private async Task BookSlotAsync(string sessionId, string token)
        {
            var scheduleAppointmentReq = new StringContent(JsonConvert.SerializeObject(new ScheduleAppointmentRequest
            {
                tkn = _umangtoken,
                usrid = _userId,
                token = token,
                session_id = sessionId,
                beneficiaries = _beneficiaries
            }), Encoding.UTF8, Constants.ApplicationJson);
            var httpRes = await _cowinHttpClient.PostAsync(Constants.URLs.Cowin + Constants.URLs.Verbs.ScheduleAppointment, scheduleAppointmentReq, tokenSource.Token);
            if (httpRes.IsSuccessStatusCode)
            {
                BaseResponse scheduleAppointmentRes = null;
                try
                {
                    scheduleAppointmentRes = JsonConvert.DeserializeObject<BaseResponse>(await httpRes.Content.ReadAsStringAsync());
                }
                catch
                {
                    //The exceptions are suppressed to not waste time in case of a failure and move on to the next retry.
                }
                if (scheduleAppointmentRes != null)
                {
                    if (IsReponseValid(scheduleAppointmentRes) &&
                        scheduleAppointmentRes.pd.appointment_confirmation_no != null)
                    {
                        _isSlotBooked = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nSuccess! Slot booked succeesfuly.");
                        Console.ResetColor();
                        tokenSource.Cancel();
                    }
                    else if (scheduleAppointmentRes.pd != null && scheduleAppointmentRes.pd == "401 Unauthorized: [Unauthenticated access!]")
                    {
                        _isSlotBooked = true;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nUnauthenticated access! Please rerun the program.");
                        Console.ResetColor();
                    }
                }
            }
        }

        private bool IsReponseValid(BaseResponse response)
        {
            return response != null &&
                response.rc != null &&
                !string.IsNullOrWhiteSpace(response.rc) &&
                response.rc == "200" &&
                response.rd != null &&
                !string.IsNullOrWhiteSpace(response.rd) &&
                response.rd == "Success." &&
                response.rs != null &&
                !string.IsNullOrWhiteSpace(response.rs) &&
                response?.rs == "S" &&
                response.pd != null;
        }
    }
}
