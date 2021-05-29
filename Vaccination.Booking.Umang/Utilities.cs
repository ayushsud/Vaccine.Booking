using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccination.Booking.Umang
{
    public static class Utilities
    {
        public static async Task<HttpResponseMessage> PostWithRetryAsync(HttpClient httpClient, string url, HttpContent request)
        {
            HttpResponseMessage response = null;
            bool success = false;
            do
            {
                try
                {
                    response = await httpClient.PostAsync(url, request);
                }
                catch
                {
                    //The exceptions are suppressed to not waste time in case of a failure and move on to the next retry.
                }
                if (response != null && response.IsSuccessStatusCode)
                {
                    success = true;
                }
                else
                {
                    await Task.Delay(700);
                }
            }
            while (!success);
            return response;
        }

        public static async Task<HttpResponseMessage> PostWithRetryAsync(HttpClient httpClient, string url, HttpContent request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            bool success = false;
            do
            {
                try
                {
                    response = await httpClient.PostAsync(url, request, cancellationToken);
                }
                catch(TaskCanceledException)
                {
                    throw;
                }
                catch
                {
                    //The exceptions are suppressed to not waste time in case of a failure and move on to the next retry.
                }
                if (response != null && response.IsSuccessStatusCode)
                {
                    success = true;
                }
                else
                {
                    await Task.Delay(700);
                }
            }
            while (!success);
            return response;
        }

        public static bool IsReponseValid(BaseResponse response)
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
