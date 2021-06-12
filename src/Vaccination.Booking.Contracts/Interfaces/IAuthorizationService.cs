using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vaccination.Booking.Contracts
{
    public interface IAuthorizationService
    {
        Task<bool> Authorize(string password);
    }
}
