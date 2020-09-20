using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserAsync(long Id);
        Task<string> UpdateDisplayNameAsync(long userId, string newDisplayName);
    }
}
