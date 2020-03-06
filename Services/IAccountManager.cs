using ChatyChaty.Model.AccountModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public interface IAccountManager
    {
        Task<AuthenticationResult> CreateAccount(AccountModel accountModel);
        Task<AuthenticationResult> Login(AccountModel accountModel);
    }
}
