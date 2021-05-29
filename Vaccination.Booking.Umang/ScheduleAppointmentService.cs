using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccination.Booking.Umang
{
    public class ScheduleAppointmentService : IScheduleAppointmentService
    {
        private bool _isSlotBooked;
        private string _umangtoken;
        private string _userId;
        private List<string> _centerPriorityList;
        private List<string> _beneficiaries;
        private readonly CancellationTokenSource tokenSource;
        private readonly IProfileService _profileService;
        private readonly IPinCodeProvider _pinCodeProvider;
        private readonly IUmangTokenProvider _umangTokenProvider;

        public ScheduleAppointmentService(IProfileService profileService, IPinCodeProvider pinCodeProvider, IUmangTokenProvider umangTokenProvider)
        {
            _profileService = profileService;
            _pinCodeProvider = pinCodeProvider;
            _umangTokenProvider = umangTokenProvider;
            _isSlotBooked = false;
            tokenSource = new CancellationTokenSource();
        }

        public async Task ScheduleAppointmentAsync()
        {
            var profile = _profileService.GetProfile();
            _centerPriorityList = _pinCodeProvider.GetPinCodes();
            _beneficiaries = profile.Beneficiaries;
            if (string.IsNullOrWhiteSpace(profile.Mobile) ||
                string.IsNullOrWhiteSpace(profile.Mpin) ||
                string.IsNullOrWhiteSpace(profile.Date) ||
                profile.DistrictId < 1 ||
                _beneficiaries.Count == 0 ||
                _centerPriorityList.Count == 0)
                return;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Constants.URLs.BaseUrl);
                #region add headers
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.BearerTokens.Cowin);
                httpClient.DefaultRequestHeaders.Connection.Clear();
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
                httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                httpClient.DefaultRequestHeaders.Add("deptid", "355");
                httpClient.DefaultRequestHeaders.Add("formtrkr", "0");
                httpClient.DefaultRequestHeaders.Add("srvid", "1604");
                httpClient.DefaultRequestHeaders.Add("subsid", "0");
                httpClient.DefaultRequestHeaders.Add("subsid2", "0");
                httpClient.DefaultRequestHeaders.Add("tenantId", string.Empty);
                #endregion
                var umangLoginResponse = await _umangTokenProvider.GetUmangToken(profile.Mobile, profile.Mpin);
                if (!string.IsNullOrWhiteSpace(umangLoginResponse.Token) && !string.IsNullOrWhiteSpace(umangLoginResponse.UserId))
                {
                    _umangtoken = umangLoginResponse.Token;
                    _userId = umangLoginResponse.UserId;
                    var token = await GetToken(httpClient, profile.Mobile);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        Console.WriteLine("\nTrying to book slots...");
                        while (!_isSlotBooked)
                        {
                            var slotsReq = new StringContent(JsonConvert.SerializeObject(new GetSlotsRequest
                            {
                                token = token,
                                tkn = _umangtoken,
                                usrid = _userId,
                                district_id = profile.DistrictId,
                                date = profile.Date
                            }), Encoding.UTF8, Constants.ApplicationJson);
                            var slotsRes = await Utilities.PostWithRetryAsync(httpClient, Constants.URLs.Cowin + Constants.URLs.Verbs.GetSlots, slotsReq, tokenSource.Token);
                            if (slotsRes.IsSuccessStatusCode)
                            {
                                var slotsData = JsonConvert.DeserializeObject<GetSlotsResponse>(await slotsRes.Content.ReadAsStringAsync());
                                if (Utilities.IsReponseValid(new BaseResponse
                                {
                                    rc = slotsData.rc,
                                    rd = slotsData.rd,
                                    rs = slotsData.rs,
                                    pd = slotsData.pd
                                }))
                                {
                                    if (slotsData.pd.centers.Count > 0)
                                    {
                                        await FindEligibleCentersAsync(slotsData.pd.centers, httpClient, token, tokenSource.Token);
                                    }
                                    else
                                        await Task.Delay(500);
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task<string> GetToken(HttpClient httpClient, string mobile)
        {
            var generateOtpRequest = new StringContent(JsonConvert.SerializeObject(new GenerateOtpRequest
            {
                tkn = _umangtoken,
                usrid = _userId,
                mobile = mobile
            }), Encoding.UTF8, Constants.ApplicationJson);
            var generateOtpResponse = await Utilities.PostWithRetryAsync(httpClient, Constants.URLs.Cowin + Constants.URLs.Verbs.GenerateOtp, generateOtpRequest);
            if (generateOtpResponse.IsSuccessStatusCode)
            {
                var generateOtpData = JsonConvert.DeserializeObject<BaseResponse>(await generateOtpResponse.Content.ReadAsStringAsync());
                if (Utilities.IsReponseValid(generateOtpData))
                {
                    string txnId = generateOtpData.pd.txnId;
                    Console.WriteLine("Enter OTP");
                    var otp = Console.ReadLine();
                    var confirmOtpRequest = new StringContent(JsonConvert.SerializeObject(new ConfirmOtpRequest
                    {
                        txnId = txnId,
                        otp = otp,
                        tkn = _umangtoken,
                        usrid = _userId
                    }), Encoding.UTF8, Constants.ApplicationJson);
                    var confirmOtpResponse = await Utilities.PostWithRetryAsync(httpClient, Constants.URLs.Cowin + Constants.URLs.Verbs.ConfirmOtp, confirmOtpRequest);
                    if (confirmOtpResponse.IsSuccessStatusCode)
                    {
                        var confirmOtpData = JsonConvert.DeserializeObject<BaseResponse>(await confirmOtpResponse.Content.ReadAsStringAsync());
                        if (Utilities.IsReponseValid(confirmOtpData))
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

        private async Task FindEligibleCentersAsync(List<Center> centers, HttpClient httpClient, string token, CancellationToken cancellationToken)
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
                        tasks.Add(BookSlotAsync(httpClient, x.session_id, token, cancellationToken));
                    }));
                }
            });
            if (tasks.Count == 0)
                await Task.Delay(500);
            await Task.WhenAll(tasks);
        }

        private async Task BookSlotAsync(HttpClient httpClient, string sessionId, string token, CancellationToken cancellationToken)
        {
            var scheduleAppointmentReq = new StringContent(JsonConvert.SerializeObject(new ScheduleAppointmentRequest
            {
                tkn = _umangtoken,
                usrid = _userId,
                token = token,
                session_id = sessionId,
                beneficiaries = _beneficiaries
            }), Encoding.UTF8, Constants.ApplicationJson);
            var httpRes = await httpClient.PostAsync(Constants.URLs.Cowin + Constants.URLs.Verbs.ScheduleAppointment, scheduleAppointmentReq, cancellationToken);
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
                    if (Utilities.IsReponseValid(scheduleAppointmentRes) &&
                        scheduleAppointmentRes.pd.appointment_confirmation_no != null)
                    {
                        this._isSlotBooked = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nSuccess! Slot booked succeesfuly.");
                        Console.ResetColor();
                        tokenSource.Cancel();
                    }
                    else if (scheduleAppointmentRes.pd != null && scheduleAppointmentRes.pd == "401 Unauthorized: [Unauthenticated access!]")
                    {
                        _isSlotBooked = true;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nUnauthenticated access! Pease rerun the program.");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}
