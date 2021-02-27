using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.InfastructureInterfaces
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserAsync(UserId Id);
        Task<string> UpdateDisplayNameAsync(UserId userId, string newDisplayName);
    }
}
