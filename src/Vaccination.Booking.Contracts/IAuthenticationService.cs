using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vaccination.Booking.Contracts
{
    public interface IAuthenticationService
    {
        Task<bool> Authenticate(string password);
    }
}
