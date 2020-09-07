using ChatyChaty.Model.AccountModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services.AuthenticationManager
{
    /// <summary>
    /// Interface that handle authentication and security related logic
    /// </summary>
    public interface IAuthenticationManager
    {
        Task<AuthenticationResult> CreateAccount(string username, string password, string displayName);
        Task<AuthenticationResult> Login(string username, string password);
        Task<AuthenticationResult> ChangePassword(string userName, string currentPassword, string newPassword);
    }
}
