using ChatyChaty.Model;
using ChatyChaty.Model.AccountModel;
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
        Task<AppUser> GetUser(long UserId);
        Task<string> UpdateDisplayName(long UserId, string NewDisplayName);
    }
}
