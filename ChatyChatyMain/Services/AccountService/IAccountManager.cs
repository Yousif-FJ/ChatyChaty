using ChatyChaty.Model;
using ChatyChaty.Model.AuthenticationModel;
using ChatyChaty.Model.DBModel;
using Microsoft.AspNetCore.Identity;
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
        Task<AppUser> GetUser(string UserName);

    }
}
