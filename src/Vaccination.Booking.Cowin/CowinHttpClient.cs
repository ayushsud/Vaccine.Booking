using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vaccination.Booking.Cowin.Contracts;

namespace Vaccination.Booking.Cowin
{
    public class CowinHttpClient : ICowinHttpClient
    {
        private readonly HttpClient _httpClient;
        public CowinHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken)
        {
            return _httpClient.GetAsync(url, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent request, CancellationToken cancellationToken)
        {
            return _httpClient.PostAsync(url, request, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent request)
        {
            return _httpClient.PostAsync(url, request);
        }

        public void ClearAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public void AddAuthorizationHeader(string scheme, string parameter)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        }
    }
}
