using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccination.Booking.Umang
{
    public class UmangTokenProvider : IUmangTokenProvider
    {
        public async Task<UmangLoginResponse> GetUmangToken(string mobile, string mpin)
        {
            Console.WriteLine("Generating Umang Token");
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Constants.URLs.BaseUrl);
                #region add headers
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.BearerTokens.Umang);
                httpClient.DefaultRequestHeaders.Connection.Clear();
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
                httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                #endregion
                var req = new StringContent(JsonConvert.SerializeObject(new UmangLoginRequest
                {
                    mno = mobile,
                    lid = mpin
                }), Encoding.UTF8, Constants.ApplicationJson);
                var httpRes = await Utilities.PostWithRetryAsync(httpClient, Constants.URLs.Core + Constants.URLs.Verbs.UmangLogin, req);
                if (httpRes.IsSuccessStatusCode)
                {
                    var tokenRes = JsonConvert.DeserializeObject<BaseResponse>(await httpRes.Content.ReadAsStringAsync());
                    if (tokenRes.pd != null && tokenRes.pd.tkn != null && tokenRes.pd.generalpd?.uid != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Umang Login Successful");
                        Console.ResetColor();
                        return new UmangLoginResponse
                        {
                            Token = tokenRes.pd?.tkn,
                            UserId = tokenRes.pd?.generalpd?.uid
                        };
                    }
                }
            }
            return default;
        }
    }
}
