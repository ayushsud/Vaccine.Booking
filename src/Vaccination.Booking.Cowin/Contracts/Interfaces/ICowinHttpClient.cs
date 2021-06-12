using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vaccination.Booking.Cowin.Contracts
{
    public interface ICowinHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent request, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent request);
        void AddAuthorizationHeader(string scheme, string parameter);
        void ClearAuthorizationHeader();
    }
}
