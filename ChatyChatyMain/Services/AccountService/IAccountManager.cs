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
        Task<AppUser> GetUser(string UserName);
        Task<string> UpdateDisplayName(long UserId, string NewDisplayName);
    }
}
