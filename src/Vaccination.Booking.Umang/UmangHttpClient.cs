using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Vaccination.Booking.Umang.Contracts;

namespace Vaccination.Booking.Umang
{
    public class UmangHttpClient : BaseHttpClient, IUmangHttpClient
    {
        public UmangHttpClient(HttpClient httpClient) : base(httpClient) { }
    }
}
