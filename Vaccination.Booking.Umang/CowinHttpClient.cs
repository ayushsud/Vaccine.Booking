using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccination.Booking.Umang
{
    public class CowinHttpClient : BaseHttpClient, ICowinHttpClient
    {
        public CowinHttpClient(HttpClient httpClient) : base(httpClient) { }
    }
}
