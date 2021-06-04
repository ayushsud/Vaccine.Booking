using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vaccination.Booking.Umang.Contracts
{
    public interface IBaseHttpClient
    {
        Task<HttpResponseMessage> PostAsync(string url, HttpContent request, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostWithRetryAsync(string url, HttpContent request, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostWithRetryAsync(string url, HttpContent request);
    }
}
