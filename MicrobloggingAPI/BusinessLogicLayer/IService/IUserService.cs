using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(User user);
        Task<string> AuthenticateUserAsync(string email, string password);
    }
}
