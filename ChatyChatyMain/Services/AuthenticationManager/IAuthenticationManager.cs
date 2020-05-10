using ChatyChaty.Model.AccountModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public interface IAuthenticationManager
    {
        Task<AuthenticationResult> CreateAccount(string username, string password, string displayName);
        Task<AuthenticationResult> Login(string username, string password);
        Task<AuthenticationResult> ChangePassword(string userName, string currentPassword, string newPassword);
    }
}
