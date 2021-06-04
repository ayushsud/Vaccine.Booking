using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccination.Booking.Umang
{
    public class BaseHttpClient: IBaseHttpClient
    {
        private readonly HttpClient _httpClient;
        public BaseHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent request, CancellationToken cancellationToken)
        {
            return await _httpClient.PostAsync(url, request, cancellationToken);
        }

        public async Task<HttpResponseMessage> PostWithRetryAsync(string url, HttpContent request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            bool success = false;
            do
            {
                try
                {
                    response = await _httpClient.PostAsync(url, request, cancellationToken);
                }
                catch (TaskCanceledException)
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

        public async Task<HttpResponseMessage> PostWithRetryAsync(string url, HttpContent request)
        {
            HttpResponseMessage response = null;
            bool success = false;
            do
            {
                try
                {
                    response = await _httpClient.PostAsync(url, request);
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
    }
}
