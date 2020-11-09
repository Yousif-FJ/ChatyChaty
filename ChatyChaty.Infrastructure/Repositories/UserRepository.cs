using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Infrastructure.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatyChatyContext dBContext;

        public UserRepository(ChatyChatyContext DBContext)
        {
            dBContext = DBContext;
        }

        public async Task<string> UpdateDisplayNameAsync(long UserId, string NewDisplayName)
        {
            var User = await dBContext.Users.FindAsync(UserId);
            User.DisplayName = NewDisplayName;
            var Updated = dBContext.Users.Update(User);
            await dBContext.SaveChangesAsync();
            return Updated.Entity.DisplayName;

        }

        public async Task<AppUser> GetUserAsync(long Id)
        {
            return await dBContext.Users.FindAsync(Id);
        }
    }
}
