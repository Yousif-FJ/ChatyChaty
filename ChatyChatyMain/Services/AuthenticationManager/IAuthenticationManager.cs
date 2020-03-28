using ChatyChaty.Model.AccountModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public interface IAuthenticationManager
    {
        Task<AuthenticationResult> CreateAccount(AccountModel accountModel);
        Task<AuthenticationResult> Login(AccountModel accountModel);
    }
}
